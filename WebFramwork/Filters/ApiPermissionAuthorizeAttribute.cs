using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Services.Contracts.Authentications;
using System.Net;
using System.Security.Claims;
using WebFramework.Api;

namespace WebFramework.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiPermissionAuthorizeAttribute(params string[] requiredPermissions) : Attribute, IAsyncActionFilter
{
    private readonly string[] _requiredPermissions = requiredPermissions;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userContext = context.HttpContext.RequestServices.GetService<IUserContext>();
        if (userContext == null)
        {
            context.Result = new JsonResult(new ApiResult<string>(false, HttpStatusCode.Unauthorized, string.Empty, "دسترسی مجاز نیست"))
            { StatusCode = (int)HttpStatusCode.Unauthorized };
            return;
        }

        if (userContext.UserId == Guid.Empty)
        {
            var authorization = context.HttpContext.Request.Headers.Authorization.ToString();
            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authorization["Bearer ".Length..].Trim();
                var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
                var claimsPrincipal = jwtService?.Validate(token);
                var userIdRaw = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdRaw, out var parsedUserId))
                    userContext.UserId = parsedUserId;
            }
        }

        if (userContext.UserId == Guid.Empty)
        {
            context.Result = new JsonResult(new ApiResult<string>(false, HttpStatusCode.Unauthorized, string.Empty, "دسترسی مجاز نیست"))
            { StatusCode = (int)HttpStatusCode.Unauthorized };
            return;
        }

        var authzService = context.HttpContext.RequestServices.GetService<IEffectiveAuthorizationService>();
        if (authzService == null)
        {
            context.Result = new JsonResult(new ApiResult<string>(false, HttpStatusCode.Forbidden, string.Empty, "دسترسی مجاز نیست"))
            { StatusCode = (int)HttpStatusCode.Forbidden };
            return;
        }

        var effective = await authzService.GetEffectiveAuthorizationAsync(userContext.UserId, context.HttpContext.RequestAborted);
        var hasAllPermissions = _requiredPermissions.All(p => effective.Permissions.Contains(p, StringComparer.OrdinalIgnoreCase));
        if (!hasAllPermissions)
        {
            context.Result = new JsonResult(new ApiResult<string>(false, HttpStatusCode.Forbidden, string.Empty, "شما دسترسی لازم برای این عملیات را ندارید"))
            { StatusCode = (int)HttpStatusCode.Forbidden };
            return;
        }

        await next();
    }
}
