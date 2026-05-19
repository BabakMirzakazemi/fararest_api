using AutoMapper;
using Common.Markers;
using Entities.Categories;
using Entities.Items;
using Entities.Menus;
using Services.DTOs.Common;

namespace Services.DTOs.Menus;

public sealed class CreateMenuRequest
{
    public long OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MenuEntityStatus Status { get; set; } = MenuEntityStatus.Active;
}

public sealed class UpdateMenuRequest
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MenuEntityStatus Status { get; set; } = MenuEntityStatus.Active;
}

public sealed class SearchMenusRequest : PagingRequest
{
    public long? OrganizationId { get; set; }
    public string? Query { get; set; }
    public MenuEntityStatus? Status { get; set; }
    public bool IncludeArchived { get; set; }
}

public sealed class CreateCategoryRequest
{
    public long MenuId { get; set; }
    public long? ParentCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string[]? ImageUrls { get; set; }
    public MenuEntityStatus Status { get; set; } = MenuEntityStatus.Active;
}

public sealed class UpdateCategoryRequest
{
    public long Id { get; set; }
    public long? ParentCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string[]? ImageUrls { get; set; }
    public MenuEntityStatus Status { get; set; } = MenuEntityStatus.Active;
}

public sealed class SearchCategoriesRequest : PagingRequest
{
    public long? MenuId { get; set; }
    public long? ParentCategoryId { get; set; }
    public string? Query { get; set; }
    public MenuEntityStatus? Status { get; set; }
    public bool IncludeArchived { get; set; }
}

public sealed class CreateItemRequest
{
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public MenuItemType ItemType { get; set; }
    public decimal PriceAmount { get; set; }
    public string? Description { get; set; }
    public string[]? ImageUrls { get; set; }
    public MenuEntityStatus Status { get; set; } = MenuEntityStatus.Active;
}

public sealed class UpdateItemRequest
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public MenuItemType ItemType { get; set; }
    public decimal PriceAmount { get; set; }
    public string? Description { get; set; }
    public string[]? ImageUrls { get; set; }
    public MenuEntityStatus Status { get; set; } = MenuEntityStatus.Active;
}

public sealed class SearchItemsRequest : PagingRequest
{
    public long? MenuId { get; set; }
    public long? CategoryId { get; set; }
    public string? Query { get; set; }
    public MenuEntityStatus? Status { get; set; }
    public bool IncludeArchived { get; set; }
}

public sealed class MenuDto : IHaveCustomMapping
{
    public long Id { get; set; }
    public long OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MenuEntityStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public void CreateMappings(Profile profile)
    {
        profile.CreateMap<Menu, MenuDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.IsDeleted
                ? MenuEntityStatus.Archived
                : s.IsActive ? MenuEntityStatus.Active : MenuEntityStatus.Inactive));
    }
}

public sealed class MenuListItemDto : IHaveCustomMapping
{
    public long Id { get; set; }
    public long OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public MenuEntityStatus Status { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public void CreateMappings(Profile profile)
    {
        profile.CreateMap<Menu, MenuListItemDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.IsDeleted
                ? MenuEntityStatus.Archived
                : s.IsActive ? MenuEntityStatus.Active : MenuEntityStatus.Inactive));
    }
}

public sealed class CategoryDto : IHaveCustomMapping
{
    public long Id { get; set; }
    public long OrganizationId { get; set; }
    public long MenuId { get; set; }
    public long? ParentCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string[]? ImageUrls { get; set; }
    public MenuEntityStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public void CreateMappings(Profile profile)
    {
        profile.CreateMap<Category, CategoryDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.IsDeleted
                ? MenuEntityStatus.Archived
                : s.IsActive ? MenuEntityStatus.Active : MenuEntityStatus.Inactive));
    }
}

public sealed class CategoryListItemDto : IHaveCustomMapping
{
    public long Id { get; set; }
    public long OrganizationId { get; set; }
    public long MenuId { get; set; }
    public long? ParentCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public MenuEntityStatus Status { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public void CreateMappings(Profile profile)
    {
        profile.CreateMap<Category, CategoryListItemDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.IsDeleted
                ? MenuEntityStatus.Archived
                : s.IsActive ? MenuEntityStatus.Active : MenuEntityStatus.Inactive));
    }
}

public sealed class ItemDto : IHaveCustomMapping
{
    public long Id { get; set; }
    public long OrganizationId { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public MenuItemType ItemType { get; set; }
    public decimal PriceAmount { get; set; }
    public string? Description { get; set; }
    public string[]? ImageUrls { get; set; }
    public MenuEntityStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public void CreateMappings(Profile profile)
    {
        profile.CreateMap<Item, ItemDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.IsDeleted
                ? MenuEntityStatus.Archived
                : s.IsActive ? MenuEntityStatus.Active : MenuEntityStatus.Inactive));
    }
}

public sealed class ItemListItemDto : IHaveCustomMapping
{
    public long Id { get; set; }
    public long OrganizationId { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public MenuItemType ItemType { get; set; }
    public decimal PriceAmount { get; set; }
    public MenuEntityStatus Status { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public void CreateMappings(Profile profile)
    {
        profile.CreateMap<Item, ItemListItemDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.IsDeleted
                ? MenuEntityStatus.Archived
                : s.IsActive ? MenuEntityStatus.Active : MenuEntityStatus.Inactive));
    }
}
