using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using WebFramework.Api;

namespace WebFramework.Filters.FluentValidationFilters;

[AttributeUsage(AttributeTargets.Class)]
public class FluentValidationFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.FirstOrDefault().Value;
        if (request != null)
        {
            var requestType = request.GetType();
            var validationType = typeof(IValidator<>).MakeGenericType(requestType);
            var validationContext = new ValidationContext<object>(request);
            var validationService = context.HttpContext.RequestServices.GetService(validationType);
            if (validationService != null && validationService is IValidator validator)
            {
                var result = await validator.ValidateAsync(validationContext);
                if (!result.IsValid)
                {
                    context.Result =
                            new JsonResult(new ApiResult(false, HttpStatusCode.BadRequest, null, result.Errors.BuildValidationErrors()))
                            { StatusCode = 400 };
                }
            }
        }

        await base.OnActionExecutionAsync(context, next);
    }
}
