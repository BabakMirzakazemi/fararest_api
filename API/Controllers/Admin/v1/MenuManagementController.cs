using Entities.Users;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts.Menus;
using Services.DTOs.Common;
using Services.DTOs.Menus;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.Admin.v1;

[ApiVersion("1")]
public sealed class MenuManagementController(IMenuManagementService menuManagementService) : BaseAdminApiController
{
    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<MenuDto> CreateMenuAsync(CreateMenuRequest request, CancellationToken cancellationToken)
        => await menuManagementService.CreateMenuAsync(request, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<MenuDto> UpdateMenuAsync(UpdateMenuRequest request, CancellationToken cancellationToken)
        => await menuManagementService.UpdateMenuAsync(request, cancellationToken);

    [HttpGet("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<MenuDto> GetMenuAsync([FromQuery] long id, CancellationToken cancellationToken)
        => await menuManagementService.GetMenuAsync(id, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<PagingDTO<MenuListItemDto>> SearchMenusAsync(SearchMenusRequest request, CancellationToken cancellationToken)
        => await menuManagementService.SearchMenusAsync(request, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task ArchiveMenuAsync([FromQuery] long id, CancellationToken cancellationToken)
        => await menuManagementService.ArchiveMenuAsync(id, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
        => await menuManagementService.CreateCategoryAsync(request, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
        => await menuManagementService.UpdateCategoryAsync(request, cancellationToken);

    [HttpGet("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<CategoryDto> GetCategoryAsync([FromQuery] long id, CancellationToken cancellationToken)
        => await menuManagementService.GetCategoryAsync(id, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<PagingDTO<CategoryListItemDto>> SearchCategoriesAsync(SearchCategoriesRequest request, CancellationToken cancellationToken)
        => await menuManagementService.SearchCategoriesAsync(request, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task ArchiveCategoryAsync([FromQuery] long id, CancellationToken cancellationToken)
        => await menuManagementService.ArchiveCategoryAsync(id, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<ItemDto> CreateItemAsync(CreateItemRequest request, CancellationToken cancellationToken)
        => await menuManagementService.CreateItemAsync(request, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<ItemDto> UpdateItemAsync(UpdateItemRequest request, CancellationToken cancellationToken)
        => await menuManagementService.UpdateItemAsync(request, cancellationToken);

    [HttpGet("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<ItemDto> GetItemAsync([FromQuery] long id, CancellationToken cancellationToken)
        => await menuManagementService.GetItemAsync(id, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<PagingDTO<ItemListItemDto>> SearchItemsAsync(SearchItemsRequest request, CancellationToken cancellationToken)
        => await menuManagementService.SearchItemsAsync(request, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task ArchiveItemAsync([FromQuery] long id, CancellationToken cancellationToken)
        => await menuManagementService.ArchiveItemAsync(id, cancellationToken);
}
