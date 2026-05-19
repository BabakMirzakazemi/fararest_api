using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using Common.Markers;
using Entities.Accounts;
using Entities.Categories;
using Entities.Common;
using Entities.Items;
using Entities.Menus;
using Microsoft.EntityFrameworkCore;
using Services.Contracts.Menus;
using Services.Contracts.Repositories;
using Services.DTOs.Common;
using Services.DTOs.Menus;

namespace Services.Services.Menus;

public sealed class MenuManagementService(
    IMapper mapper,
    IRepository<AccountOrganization> organizationRepository,
    IRepository<Menu> menuRepository,
    IRepository<Category> categoryRepository,
    IRepository<Item> itemRepository) : IMenuManagementService, IScopedDependency
{
    public async Task<MenuDto> CreateMenuAsync(CreateMenuRequest request, CancellationToken cancellationToken)
    {
        await EnsureOrganizationExistsAsync(request.OrganizationId, cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var entity = new Menu
        {
            OrganizationId = request.OrganizationId,
            Name = NormalizeName(request.Name),
            Description = NormalizeNullable(request.Description),
            IsActive = request.Status == MenuEntityStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        await menuRepository.AddAsync(entity, cancellationToken);
        return await GetMenuAsync(entity.Id, cancellationToken);
    }

    public async Task<MenuDto> UpdateMenuAsync(UpdateMenuRequest request, CancellationToken cancellationToken)
    {
        var entity = await menuRepository.Table
            .TagWith("MenuManagement.Menus.Update")
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Menu '{request.Id}' was not found.");

        EnsureNotArchived(entity, "menu");
        entity.Name = NormalizeName(request.Name);
        entity.Description = NormalizeNullable(request.Description);
        entity.IsActive = request.Status == MenuEntityStatus.Active;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await menuRepository.UpdateAsync(entity, cancellationToken);
        return await GetMenuAsync(entity.Id, cancellationToken);
    }

    public async Task<MenuDto> GetMenuAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await menuRepository.TableNoTracking
            .TagWith("MenuManagement.Menus.Get")
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Menu '{id}' was not found.");

        return mapper.Map<MenuDto>(entity);
    }

    public async Task<PagingDTO<MenuListItemDto>> SearchMenusAsync(SearchMenusRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Menu> query = menuRepository.TableNoTracking
            .TagWith("MenuManagement.Menus.Search");

        if (request.OrganizationId.HasValue)
            query = query.Where(x => x.OrganizationId == request.OrganizationId.Value);

        if (!request.IncludeArchived)
            query = query.Where(x => !x.IsDeleted);

        if (request.Status.HasValue)
            query = ApplyStatusFilter(query, request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var queryText = request.Query.Trim();
            query = query.Where(x => x.Name.Contains(queryText) || (x.Description != null && x.Description.Contains(queryText)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var page = await query
            .OrderByDescending(x => x.UpdatedAt)
            .ThenByDescending(x => x.Id)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<MenuListItemDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagingDTO<MenuListItemDto>(page, request, totalCount);
    }

    public async Task ArchiveMenuAsync(long id, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.Table
            .TagWith("MenuManagement.Menus.Archive")
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Menu '{id}' was not found.");

        if (menu.IsDeleted)
            return;

        var now = DateTimeOffset.UtcNow;
        menu.IsDeleted = true;
        menu.IsActive = false;
        menu.UpdatedAt = now;

        var categoryIds = await categoryRepository.Table
            .Where(x => x.MenuId == id && !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var categories = await categoryRepository.Table
            .Where(x => categoryIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        foreach (var category in categories)
        {
            category.IsDeleted = true;
            category.IsActive = false;
            category.UpdatedAt = now;
        }

        var items = await itemRepository.Table
            .Where(x => categoryIds.Contains(x.CategoryId) && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            item.IsDeleted = true;
            item.IsActive = false;
            item.UpdatedAt = now;
        }

        await menuRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.TableNoTracking
            .TagWith("MenuManagement.Categories.Create.ValidateMenu")
            .FirstOrDefaultAsync(x => x.Id == request.MenuId, cancellationToken)
            ?? throw new NotFoundException($"Menu '{request.MenuId}' was not found.");

        EnsureNotArchived(menu, "menu");
        await EnsureOrganizationExistsAsync(menu.OrganizationId, cancellationToken);
        await ValidateParentCategoryAsync(menu.Id, request.ParentCategoryId, cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var entity = new Category
        {
            OrganizationId = menu.OrganizationId,
            MenuId = menu.Id,
            ParentCategoryId = request.ParentCategoryId,
            Name = NormalizeName(request.Name),
            Description = NormalizeNullable(request.Description),
            ImageUrls = NormalizeImageUrls(request.ImageUrls),
            IsActive = request.Status == MenuEntityStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        await categoryRepository.AddAsync(entity, cancellationToken);
        return await GetCategoryAsync(entity.Id, cancellationToken);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var entity = await categoryRepository.Table
            .TagWith("MenuManagement.Categories.Update")
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Category '{request.Id}' was not found.");

        EnsureNotArchived(entity, "category");
        await EnsureOrganizationExistsAsync(entity.OrganizationId, cancellationToken);
        await ValidateParentCategoryAsync(entity.MenuId, request.ParentCategoryId, cancellationToken, entity.Id);

        entity.ParentCategoryId = request.ParentCategoryId;
        entity.Name = NormalizeName(request.Name);
        entity.Description = NormalizeNullable(request.Description);
        entity.ImageUrls = NormalizeImageUrls(request.ImageUrls);
        entity.IsActive = request.Status == MenuEntityStatus.Active;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await categoryRepository.UpdateAsync(entity, cancellationToken);
        return await GetCategoryAsync(entity.Id, cancellationToken);
    }

    public async Task<CategoryDto> GetCategoryAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await categoryRepository.TableNoTracking
            .TagWith("MenuManagement.Categories.Get")
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Category '{id}' was not found.");

        return mapper.Map<CategoryDto>(entity);
    }

    public async Task<PagingDTO<CategoryListItemDto>> SearchCategoriesAsync(SearchCategoriesRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Category> query = categoryRepository.TableNoTracking
            .TagWith("MenuManagement.Categories.Search");

        if (request.MenuId.HasValue)
            query = query.Where(x => x.MenuId == request.MenuId.Value);

        if (request.ParentCategoryId.HasValue)
            query = query.Where(x => x.ParentCategoryId == request.ParentCategoryId.Value);

        if (!request.IncludeArchived)
            query = query.Where(x => !x.IsDeleted);

        if (request.Status.HasValue)
            query = ApplyStatusFilter(query, request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var queryText = request.Query.Trim();
            query = query.Where(x => x.Name.Contains(queryText) || (x.Description != null && x.Description.Contains(queryText)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var page = await query
            .OrderBy(x => x.Name)
            .ThenByDescending(x => x.Id)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<CategoryListItemDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagingDTO<CategoryListItemDto>(page, request, totalCount);
    }

    public async Task ArchiveCategoryAsync(long id, CancellationToken cancellationToken)
    {
        var rootCategory = await categoryRepository.Table
            .TagWith("MenuManagement.Categories.Archive")
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Category '{id}' was not found.");

        if (rootCategory.IsDeleted)
            return;

        var now = DateTimeOffset.UtcNow;
        var categoryIds = await GetCategorySubtreeIdsAsync(id, cancellationToken);

        var categories = await categoryRepository.Table
            .Where(x => categoryIds.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var category in categories)
        {
            category.IsDeleted = true;
            category.IsActive = false;
            category.UpdatedAt = now;
        }

        var items = await itemRepository.Table
            .Where(x => categoryIds.Contains(x.CategoryId) && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            item.IsDeleted = true;
            item.IsActive = false;
            item.UpdatedAt = now;
        }

        await categoryRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<ItemDto> CreateItemAsync(CreateItemRequest request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.TableNoTracking
            .TagWith("MenuManagement.Items.Create.ValidateCategory")
            .FirstOrDefaultAsync(x => x.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Category '{request.CategoryId}' was not found.");

        EnsureNotArchived(category, "category");
        await EnsureOrganizationExistsAsync(category.OrganizationId, cancellationToken);
        var normalizedCode = NormalizeCode(request.Code);
        await EnsureUniqueItemCodeAsync(category.OrganizationId, normalizedCode, null, cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var entity = new Item
        {
            OrganizationId = category.OrganizationId,
            CategoryId = category.Id,
            Name = NormalizeName(request.Name),
            Description = NormalizeNullable(request.Description),
            Code = normalizedCode,
            ItemType = request.ItemType,
            PriceAmount = request.PriceAmount,
            ImageUrls = NormalizeImageUrls(request.ImageUrls),
            IsActive = request.Status == MenuEntityStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        await itemRepository.AddAsync(entity, cancellationToken);
        return await GetItemAsync(entity.Id, cancellationToken);
    }

    public async Task<ItemDto> UpdateItemAsync(UpdateItemRequest request, CancellationToken cancellationToken)
    {
        var entity = await itemRepository.Table
            .TagWith("MenuManagement.Items.Update")
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Item '{request.Id}' was not found.");

        EnsureNotArchived(entity, "item");

        var category = await categoryRepository.TableNoTracking
            .TagWith("MenuManagement.Items.Update.ValidateCategory")
            .FirstOrDefaultAsync(x => x.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Category '{request.CategoryId}' was not found.");

        EnsureNotArchived(category, "category");
        await EnsureOrganizationExistsAsync(category.OrganizationId, cancellationToken);

        var normalizedCode = NormalizeCode(request.Code);
        await EnsureUniqueItemCodeAsync(category.OrganizationId, normalizedCode, entity.Id, cancellationToken);

        entity.OrganizationId = category.OrganizationId;
        entity.CategoryId = category.Id;
        entity.Name = NormalizeName(request.Name);
        entity.Description = NormalizeNullable(request.Description);
        entity.Code = normalizedCode;
        entity.ItemType = request.ItemType;
        entity.PriceAmount = request.PriceAmount;
        entity.ImageUrls = NormalizeImageUrls(request.ImageUrls);
        entity.IsActive = request.Status == MenuEntityStatus.Active;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await itemRepository.UpdateAsync(entity, cancellationToken);
        return await GetItemAsync(entity.Id, cancellationToken);
    }

    public async Task<ItemDto> GetItemAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await itemRepository.TableNoTracking
            .TagWith("MenuManagement.Items.Get")
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Item '{id}' was not found.");

        return mapper.Map<ItemDto>(entity);
    }

    public async Task<PagingDTO<ItemListItemDto>> SearchItemsAsync(SearchItemsRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Item> query = itemRepository.TableNoTracking
            .TagWith("MenuManagement.Items.Search");

        if (request.CategoryId.HasValue)
            query = query.Where(x => x.CategoryId == request.CategoryId.Value);

        if (request.MenuId.HasValue)
        {
            var menuId = request.MenuId.Value;
            query = query.Where(x => categoryRepository.TableNoTracking.Any(c => c.Id == x.CategoryId && c.MenuId == menuId));
        }

        if (!request.IncludeArchived)
            query = query.Where(x => !x.IsDeleted);

        if (request.Status.HasValue)
            query = ApplyStatusFilter(query, request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var queryText = request.Query.Trim();
            query = query.Where(x => x.Name.Contains(queryText) || x.Code.Contains(queryText) || (x.Description != null && x.Description.Contains(queryText)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var page = await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Code)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ItemListItemDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagingDTO<ItemListItemDto>(page, request, totalCount);
    }

    public async Task ArchiveItemAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await itemRepository.Table
            .TagWith("MenuManagement.Items.Archive")
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Item '{id}' was not found.");

        if (entity.IsDeleted)
            return;

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await itemRepository.UpdateAsync(entity, cancellationToken);
    }

    private static IQueryable<TEntity> ApplyStatusFilter<TEntity>(IQueryable<TEntity> query, MenuEntityStatus status)
        where TEntity : BaseEntity<long>
    {
        return status switch
        {
            MenuEntityStatus.Active => query.Where(x => !x.IsDeleted && EF.Property<bool>(x, nameof(Menu.IsActive))),
            MenuEntityStatus.Inactive => query.Where(x => !x.IsDeleted && !EF.Property<bool>(x, nameof(Menu.IsActive))),
            MenuEntityStatus.Archived => query.Where(x => x.IsDeleted),
            _ => query
        };
    }

    private async Task ValidateParentCategoryAsync(long menuId, long? parentCategoryId, CancellationToken cancellationToken, long? currentCategoryId = null)
    {
        if (!parentCategoryId.HasValue)
            return;

        if (currentCategoryId.HasValue && parentCategoryId.Value == currentCategoryId.Value)
            throw new BadRequestException("A category cannot be its own parent.");

        var parent = await categoryRepository.TableNoTracking
            .TagWith("MenuManagement.Categories.ValidateParent")
            .FirstOrDefaultAsync(x => x.Id == parentCategoryId.Value, cancellationToken)
            ?? throw new NotFoundException($"Parent category '{parentCategoryId.Value}' was not found.");

        EnsureNotArchived(parent, "parent category");

        if (parent.MenuId != menuId)
            throw new BadRequestException("Parent category must belong to the same menu.");

        if (!currentCategoryId.HasValue)
            return;

        var subtreeIds = await GetCategorySubtreeIdsAsync(currentCategoryId.Value, cancellationToken);
        if (subtreeIds.Contains(parentCategoryId.Value))
            throw new BadRequestException("A category cannot be moved under one of its descendants.");
    }

    private async Task EnsureUniqueItemCodeAsync(long organizationId, string code, long? currentItemId, CancellationToken cancellationToken)
    {
        var duplicateExists = await itemRepository.TableNoTracking
            .TagWith("MenuManagement.Items.ValidateUniqueCode")
            .AnyAsync(x => x.OrganizationId == organizationId && x.Code == code && (!currentItemId.HasValue || x.Id != currentItemId.Value), cancellationToken);

        if (duplicateExists)
            throw new BadRequestException($"Item code '{code}' already exists.");
    }

    private async Task EnsureOrganizationExistsAsync(long organizationId, CancellationToken cancellationToken)
    {
        var organizationExists = await organizationRepository.TableNoTracking
            .TagWith("MenuManagement.Organizations.ValidateExists")
            .AnyAsync(x => x.Id == organizationId, cancellationToken);

        if (!organizationExists)
            throw new BadRequestException("شناسه یکتای سازمان/کسب و کار معتبر را وارد نمایید.");
    }

    private async Task<HashSet<long>> GetCategorySubtreeIdsAsync(long rootCategoryId, CancellationToken cancellationToken)
    {
        var visited = new HashSet<long> { rootCategoryId };
        var frontier = new List<long> { rootCategoryId };

        while (frontier.Count > 0)
        {
            var childIds = await categoryRepository.TableNoTracking
                .Where(x => x.ParentCategoryId.HasValue && frontier.Contains(x.ParentCategoryId.Value))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            frontier = childIds.Where(visited.Add).ToList();
        }

        return visited;
    }

    private static void EnsureNotArchived(Menu entity, string entityName)
    {
        if (entity.IsDeleted)
            throw new BadRequestException($"The {entityName} is archived.");
    }

    private static void EnsureNotArchived(Category entity, string entityName)
    {
        if (entity.IsDeleted)
            throw new BadRequestException($"The {entityName} is archived.");
    }

    private static void EnsureNotArchived(Item entity, string entityName)
    {
        if (entity.IsDeleted)
            throw new BadRequestException($"The {entityName} is archived.");
    }

    private static string NormalizeName(string value) => value.Trim();

    private static string NormalizeCode(string value) => value.Trim().ToUpperInvariant();

    private static string? NormalizeNullable(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string[]? NormalizeImageUrls(string[]? values)
    {
        if (values == null || values.Length == 0)
            return null;

        var normalized = values
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return normalized.Length == 0 ? null : normalized;
    }
}
