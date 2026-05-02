using Common.Configurations;
using Common.Markers;
using Common.Utilities.Extensions;
using Entities.Enums.Users;
using Microsoft.AspNetCore.Http;
using Services.Contracts.Authentications;
using Services.Contracts.Caching;
using Services.Contracts.Repositories;
using Services.DTOs.Admins;
using Services.DTOs.Users;
using Services.Services.Repositories;

namespace Services.Services.Authentications;

public class UserService : Repository<User>, IUserService, IScopedDependency
{
    // Cache key namespaces are centralized here to keep key contracts stable and readable.
    private const string UsersListCacheVersionKey = "users:list:version";
    private const int UsersListCacheVersionTtlHours = 168; // 7 days
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IAppCacheService _cacheService;
    private readonly CachingSettings _cachingSettings;

    public UserService(
        DbContext dbContext,
        IUserContext userContext,
        IMapper mapper,
        IAppCacheService cacheService,
        IOptions<PerformanceSettings> performanceSettings) : base(dbContext)
    {
        _userContext = userContext;
        _mapper = mapper;
        _cacheService = cacheService;
        _cachingSettings = performanceSettings.Value.Caching;
    }

    public async Task<PagingDTO<UserListDTO>> All(FilterUserRequest dto, CancellationToken cancellationToken)
    {
        // Cache key includes a version token so we can invalidate all list pages with one version bump.
        var cacheVersion = await GetUsersListCacheVersionAsync(cancellationToken);
        var cacheKey = BuildUsersListCacheKey(dto, cacheVersion);

        if (_cachingSettings.Enabled)
        {
            var cached = await _cacheService.GetAsync<PagingDTO<UserListDTO>>(cacheKey, cancellationToken);
            if (cached is not null)
                return cached;
        }

        var searchText = dto.SearchText ?? string.Empty;

        IQueryable<User> users = TableNoTracking
            // Query tag helps identify this SQL quickly in logs/query store during tuning.
            .TagWith("Users.AllPaginated")
            .Where(p =>
                string.IsNullOrEmpty(searchText) ||
                (p.Email ?? string.Empty).Contains(searchText) ||
                (p.NationalCode ?? string.Empty).Contains(searchText)
            )
            // TODO(QueryTuning): after business stabilizes, verify with real workload and add/adjust indexes
            // for this query shape (search + IsDeleted filter + Id sorting) using execution plans.
            .OrderByDescending(p => p.Id);

        int count = await users.CountAsync(cancellationToken);

        List<UserListDTO> usersList = await users
            .CreatePage(dto.PageIndex, dto.PageSize)
            .ProjectTo<UserListDTO>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var result = new PagingDTO<UserListDTO>(usersList, dto, count);

        if (_cachingSettings.Enabled)
        {
            // If a dedicated TTL is configured for users list, prefer that value; otherwise use default TTL.
            var ttl = TimeSpan.FromSeconds(_cachingSettings.UsersListTtlSeconds > 0
                ? _cachingSettings.UsersListTtlSeconds
                : _cachingSettings.DefaultTtlSeconds);

            await _cacheService.SetAsync(cacheKey, result, ttl, cancellationToken);
        }

        return result;
    }

    public async Task<CursorPagingDTO<UserListDTO>> AllByCursor(FilterUserCursorRequest dto, CancellationToken cancellationToken)
    {
        // Cursor pagination avoids expensive OFFSET scans on deep pages by seeking from last seen row.
        // Search/filter logic stays in service, while cursor mechanics are handled by generic repository method.
        var cacheVersion = await GetUsersListCacheVersionAsync(cancellationToken);
        var cacheKey = BuildUsersCursorListCacheKey(dto, cacheVersion);

        if (_cachingSettings.Enabled)
        {
            var cached = await _cacheService.GetAsync<CursorPagingDTO<UserListDTO>>(cacheKey, cancellationToken);
            if (cached is not null)
                return cached;
        }

        var searchText = dto.SearchText ?? string.Empty;
        Expression<Func<User, bool>> searchPredicate = p =>
            string.IsNullOrEmpty(searchText) ||
            (p.Email ?? string.Empty).Contains(searchText) ||
            (p.NationalCode ?? string.Empty).Contains(searchText);

        var cursorPage = await SelectByCursorAsync(
            dto,
            p => p.Id,
            cancellationToken,
            searchPredicate,
            descending: true,
            asNoTracking: true);

        var pageData = _mapper.Map<List<UserListDTO>>(cursorPage.Data);

        var result = new CursorPagingDTO<UserListDTO>(
            pageData,
            cursorPage.PageSize,
            cursorPage.HasNext,
            cursorPage.NextCursor,
            cursorPage.CurrentCursor);

        if (_cachingSettings.Enabled)
        {
            // Reuse the same endpoint-specific TTL policy used by offset paging endpoint.
            var ttl = TimeSpan.FromSeconds(_cachingSettings.UsersListTtlSeconds > 0
                ? _cachingSettings.UsersListTtlSeconds
                : _cachingSettings.DefaultTtlSeconds);

            await _cacheService.SetAsync(cacheKey, result, ttl, cancellationToken);
        }

        return result;
    }

    // Any write operation on User entity bumps cache version to invalidate all paged list keys.
    public override async Task AddAsync(User entity, CancellationToken cancellationToken, bool saveNow = true)
    {
        await base.AddAsync(entity, cancellationToken, saveNow);
        if (saveNow)
            await RefreshUsersListCacheVersionAsync(cancellationToken);
    }

    public override async Task UpdateAsync(User entity, CancellationToken cancellationToken, bool saveNow = true)
    {
        await base.UpdateAsync(entity, cancellationToken, saveNow);
        if (saveNow)
            await RefreshUsersListCacheVersionAsync(cancellationToken);
    }

    public override async Task DeleteAsync(User entity, CancellationToken cancellationToken, bool saveNow = true)
    {
        await base.DeleteAsync(entity, cancellationToken, saveNow);
        if (saveNow)
            await RefreshUsersListCacheVersionAsync(cancellationToken);
    }

    public override async Task DeleteByIdAsync(object id, CancellationToken cancellationToken, bool saveNow = true)
    {
        await base.DeleteByIdAsync(id, cancellationToken, saveNow);
        if (saveNow)
            await RefreshUsersListCacheVersionAsync(cancellationToken);
    }

    private static string BuildUsersListCacheKey(FilterUserRequest dto, string version)
    {
        var normalizedSearch = (dto.SearchText ?? string.Empty).Trim().ToLowerInvariant();
        return $"users:list:{version}:p{dto.PageIndex}:s{dto.PageSize}:q:{normalizedSearch}";
    }

    private static string BuildUsersCursorListCacheKey(FilterUserCursorRequest dto, string version)
    {
        var normalizedSearch = (dto.SearchText ?? string.Empty).Trim().ToLowerInvariant();
        var normalizedCursor = string.IsNullOrWhiteSpace(dto.Cursor) ? "first" : dto.Cursor.Trim();
        return $"users:list:{version}:cursor:{normalizedCursor}:s{dto.PageSize}:q:{normalizedSearch}";
    }

    private async Task<string> GetUsersListCacheVersionAsync(CancellationToken cancellationToken)
    {
        if (!_cachingSettings.Enabled)
            return "v-live";

        var currentVersion = await _cacheService.GetAsync<string>(UsersListCacheVersionKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(currentVersion))
            return currentVersion;

        var initialVersion = $"v-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        var versionTtl = TimeSpan.FromHours(UsersListCacheVersionTtlHours);
        await _cacheService.SetAsync(UsersListCacheVersionKey, initialVersion, versionTtl, cancellationToken);
        return initialVersion;
    }

    private async Task RefreshUsersListCacheVersionAsync(CancellationToken cancellationToken)
    {
        if (!_cachingSettings.Enabled)
            return;

        // Bumping version is safer than wildcard delete because IDistributedCache has no pattern delete contract.
        var newVersion = $"v-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var versionTtl = TimeSpan.FromHours(UsersListCacheVersionTtlHours);
        await _cacheService.SetAsync(UsersListCacheVersionKey, newVersion, versionTtl, cancellationToken);
    }
}
