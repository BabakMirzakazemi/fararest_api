using Common.Configurations;
using Common.Markers;
using Entities.Accounts;
using Microsoft.AspNetCore.Identity;
using Services.Contracts.Authentications;
using Services.Contracts.Caching;
using Services.Contracts.Repositories;
using Services.DTOs.Accounts.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace Services.Services.Authentications;

public class EffectiveAuthorizationService(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IRepository<AccountPermission> permissionRepository,
    IRepository<AccountRolePermission> rolePermissionRepository,
    IRepository<AccountPlanPermission> planPermissionRepository,
    IRepository<AccountUserPermissionGrant> userGrantRepository,
    IRepository<AccountUserPermissionRevoke> userRevokeRepository,
    IRepository<AccountUserPlanSubscription> userPlanSubscriptionRepository,
    IAppCacheService appCacheService) : IEffectiveAuthorizationService, IScopedDependency
{
    private static readonly TimeSpan AuthorizationCacheTtl = TimeSpan.FromMinutes(3);

    public async Task<CurrentUserAuthorizationResponse> GetEffectiveAuthorizationAsync(Guid userId, CancellationToken cancellationToken)
    {
        var computedVersion = await ComputePermissionVersionAsync(userId, cancellationToken);
        var cacheKey = BuildAuthorizationCacheKey(userId, computedVersion);
        var cached = await appCacheService.GetAsync<CurrentUserAuthorizationResponse>(cacheKey, cancellationToken);
        if (cached != null)
            return cached;

        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        var roles = (await userManager.GetRolesAsync(user))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var roleIdMap = await roleManager.Roles
            .Where(r => roles.Contains(r.Name!))
            .ToDictionaryAsync(r => r.Name!, r => r.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var permissionMap = await permissionRepository.TableNoTracking
            .Where(x => x.IsActive)
            .ToDictionaryAsync(x => x.Id, x => x.Key, cancellationToken);

        var rolePermissionIds = await rolePermissionRepository.TableNoTracking
            .Where(x => roleIdMap.Values.Contains(x.RoleId))
            .Select(x => x.PermissionId)
            .ToListAsync(cancellationToken);

        var activePlanIds = await userPlanSubscriptionRepository.TableNoTracking
            .Where(x => x.UserId == userId && x.IsActive && x.StartsAt <= DateTimeOffset.UtcNow && (x.EndsAt == null || x.EndsAt >= DateTimeOffset.UtcNow))
            .Select(x => x.PlanId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var planPermissionIds = await planPermissionRepository.TableNoTracking
            .Where(x => activePlanIds.Contains(x.PlanId))
            .Select(x => x.PermissionId)
            .ToListAsync(cancellationToken);

        var grantPermissionIds = await userGrantRepository.TableNoTracking
            .Where(x => x.UserId == userId)
            .Select(x => x.PermissionId)
            .ToListAsync(cancellationToken);

        var revokedPermissionIds = await userRevokeRepository.TableNoTracking
            .Where(x => x.UserId == userId)
            .Select(x => x.PermissionId)
            .ToListAsync(cancellationToken);

        var effectivePermissionIds = rolePermissionIds
            .Concat(planPermissionIds)
            .Concat(grantPermissionIds)
            .Distinct()
            .Except(revokedPermissionIds)
            .ToList();

        var permissions = effectivePermissionIds
            .Where(permissionMap.ContainsKey)
            .Select(id => permissionMap[id])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var response = new CurrentUserAuthorizationResponse(userId, roles, permissions, computedVersion);
        await appCacheService.SetAsync(cacheKey, response, AuthorizationCacheTtl, cancellationToken);
        return response;
    }

    public async Task InvalidateUserAuthorizationAsync(Guid userId, CancellationToken cancellationToken)
    {
        var version = await ComputePermissionVersionAsync(userId, cancellationToken);
        await appCacheService.RemoveAsync(BuildAuthorizationCacheKey(userId, version), cancellationToken);
    }

    private async Task<string> ComputePermissionVersionAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userRoleStamp = await userManager.Users
            .Where(x => x.Id == userId)
            .Select(x => x.SecurityStamp)
            .FirstOrDefaultAsync(cancellationToken);

        var rolePermissionStamp = await rolePermissionRepository.TableNoTracking
            .MaxAsync(x => (DateTimeOffset?)x.UpdatedAt, cancellationToken);
        var planPermissionStamp = await planPermissionRepository.TableNoTracking
            .MaxAsync(x => (DateTimeOffset?)x.UpdatedAt, cancellationToken);
        var grantStamp = await userGrantRepository.TableNoTracking
            .Where(x => x.UserId == userId)
            .MaxAsync(x => (DateTimeOffset?)x.UpdatedAt, cancellationToken);
        var revokeStamp = await userRevokeRepository.TableNoTracking
            .Where(x => x.UserId == userId)
            .MaxAsync(x => (DateTimeOffset?)x.UpdatedAt, cancellationToken);
        var planSubStamp = await userPlanSubscriptionRepository.TableNoTracking
            .Where(x => x.UserId == userId)
            .MaxAsync(x => (DateTimeOffset?)x.UpdatedAt, cancellationToken);

        var raw = $"{userId:N}|{userRoleStamp}|{rolePermissionStamp?.ToUnixTimeMilliseconds()}|{planPermissionStamp?.ToUnixTimeMilliseconds()}|{grantStamp?.ToUnixTimeMilliseconds()}|{revokeStamp?.ToUnixTimeMilliseconds()}|{planSubStamp?.ToUnixTimeMilliseconds()}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(hash).ToLowerInvariant()[..16];
    }

    private static string BuildAuthorizationCacheKey(Guid userId, string version)
        => $"authz:effective:{userId:N}:{version}";
}
