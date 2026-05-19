using Entities.Menus;
using FluentValidation;

namespace Services.DTOs.Menus;

public sealed class CreateMenuRequestValidator : AbstractValidator<CreateMenuRequest>
{
    public CreateMenuRequestValidator()
    {
        RuleFor(x => x.OrganizationId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.Description));
        RuleFor(x => x.Status).IsInEnum().NotEqual(MenuEntityStatus.Archived);
    }
}

public sealed class UpdateMenuRequestValidator : AbstractValidator<UpdateMenuRequest>
{
    public UpdateMenuRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.Description));
        RuleFor(x => x.Status).IsInEnum().NotEqual(MenuEntityStatus.Archived);
    }
}

public sealed class SearchMenusRequestValidator : AbstractValidator<SearchMenusRequest>
{
    public SearchMenusRequestValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.OrganizationId).GreaterThan(0).When(x => x.OrganizationId.HasValue);
        RuleFor(x => x.Query).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Query));
    }
}

public sealed class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.MenuId).GreaterThan(0);
        RuleFor(x => x.ParentCategoryId).GreaterThan(0).When(x => x.ParentCategoryId.HasValue);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.Description));
        RuleForEach(x => x.ImageUrls!).MaximumLength(1000).When(x => x.ImageUrls != null);
        RuleFor(x => x.Status).IsInEnum().NotEqual(MenuEntityStatus.Archived);
    }
}

public sealed class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ParentCategoryId).GreaterThan(0).When(x => x.ParentCategoryId.HasValue);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.Description));
        RuleForEach(x => x.ImageUrls!).MaximumLength(1000).When(x => x.ImageUrls != null);
        RuleFor(x => x.Status).IsInEnum().NotEqual(MenuEntityStatus.Archived);
    }
}

public sealed class SearchCategoriesRequestValidator : AbstractValidator<SearchCategoriesRequest>
{
    public SearchCategoriesRequestValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.MenuId).GreaterThan(0).When(x => x.MenuId.HasValue);
        RuleFor(x => x.ParentCategoryId).GreaterThan(0).When(x => x.ParentCategoryId.HasValue);
        RuleFor(x => x.Query).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Query));
    }
}

public sealed class CreateItemRequestValidator : AbstractValidator<CreateItemRequest>
{
    public CreateItemRequestValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ItemType).IsInEnum();
        RuleFor(x => x.PriceAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Description).MaximumLength(4000).When(x => !string.IsNullOrWhiteSpace(x.Description));
        RuleForEach(x => x.ImageUrls!).MaximumLength(1000).When(x => x.ImageUrls != null);
        RuleFor(x => x.Status).IsInEnum().NotEqual(MenuEntityStatus.Archived);
    }
}

public sealed class UpdateItemRequestValidator : AbstractValidator<UpdateItemRequest>
{
    public UpdateItemRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ItemType).IsInEnum();
        RuleFor(x => x.PriceAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Description).MaximumLength(4000).When(x => !string.IsNullOrWhiteSpace(x.Description));
        RuleForEach(x => x.ImageUrls!).MaximumLength(1000).When(x => x.ImageUrls != null);
        RuleFor(x => x.Status).IsInEnum().NotEqual(MenuEntityStatus.Archived);
    }
}

public sealed class SearchItemsRequestValidator : AbstractValidator<SearchItemsRequest>
{
    public SearchItemsRequestValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.MenuId).GreaterThan(0).When(x => x.MenuId.HasValue);
        RuleFor(x => x.CategoryId).GreaterThan(0).When(x => x.CategoryId.HasValue);
        RuleFor(x => x.Query).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Query));
    }
}
