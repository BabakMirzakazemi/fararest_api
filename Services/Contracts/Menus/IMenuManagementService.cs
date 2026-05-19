using Services.DTOs.Common;
using Services.DTOs.Menus;

namespace Services.Contracts.Menus;

public interface IMenuManagementService
{
    Task<MenuDto> CreateMenuAsync(CreateMenuRequest request, CancellationToken cancellationToken);
    Task<MenuDto> UpdateMenuAsync(UpdateMenuRequest request, CancellationToken cancellationToken);
    Task<MenuDto> GetMenuAsync(long id, CancellationToken cancellationToken);
    Task<PagingDTO<MenuListItemDto>> SearchMenusAsync(SearchMenusRequest request, CancellationToken cancellationToken);
    Task ArchiveMenuAsync(long id, CancellationToken cancellationToken);

    Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken);
    Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken cancellationToken);
    Task<CategoryDto> GetCategoryAsync(long id, CancellationToken cancellationToken);
    Task<PagingDTO<CategoryListItemDto>> SearchCategoriesAsync(SearchCategoriesRequest request, CancellationToken cancellationToken);
    Task ArchiveCategoryAsync(long id, CancellationToken cancellationToken);

    Task<ItemDto> CreateItemAsync(CreateItemRequest request, CancellationToken cancellationToken);
    Task<ItemDto> UpdateItemAsync(UpdateItemRequest request, CancellationToken cancellationToken);
    Task<ItemDto> GetItemAsync(long id, CancellationToken cancellationToken);
    Task<PagingDTO<ItemListItemDto>> SearchItemsAsync(SearchItemsRequest request, CancellationToken cancellationToken);
    Task ArchiveItemAsync(long id, CancellationToken cancellationToken);
}
