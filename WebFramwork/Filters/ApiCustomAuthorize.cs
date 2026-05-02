using Common.Configurations;
using Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Services.Contracts.Authentications;
using System.Net;
using System.Security.Claims;
using WebFramework.Api;

namespace WebFramework.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiCustomAuthorize : ActionFilterAttribute
{
    private readonly string[] _roles;
    private readonly bool _allowAnonymous;

    public ApiCustomAuthorize(bool allowAnonymous, params string[] roles)
    {
        _roles = roles;
        _allowAnonymous = allowAnonymous;
    }

    public ApiCustomAuthorize(bool allowAnonymous)
    {
        _allowAnonymous = allowAnonymous;
        _roles = [];
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var isAccessGranted = false;
        base.OnActionExecuting(context);

        var authorization = context.HttpContext.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            if (authorization.Contains("Bearer "))
            {
                var token = authorization.Replace("Bearer ", "");
                var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>()
                    ?? throw new InvalidOperationException("Jwt Service Not Resolved");

                var claimsPrincipal = jwtService.Validate(token);
                if (claimsPrincipal != null)
                {
                    var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                        ?? throw new NotFoundException("User Id Claim Not Found");
                    var userId = userIdClaim.Value;

                    var userContext = context.HttpContext.RequestServices.GetService<IUserContext>()
                        ?? throw new InvalidOperationException("User Context Not Resolved");

                    userContext.UserId = Guid.Parse(userId);

                    var roleClaims = claimsPrincipal.Claims.Where(x => x.Type == ClaimTypes.Role);
                    foreach (var roleClaim in roleClaims)
                    {
                        if (_roles.Contains(roleClaim.Value))
                        {
                            isAccessGranted = true;
                            break;
                        }
                    }
                }
            }
        }

        if (_allowAnonymous)
            isAccessGranted = true;

        if (!isAccessGranted)
            UnAuthorize(context);
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var isAccessGranted = false;
        await base.OnActionExecutionAsync(context, next);

        var authorization = context.HttpContext.Request.Headers.Authorization.ToString();

        if (!string.IsNullOrWhiteSpace(authorization))
        {
            if (authorization.Contains("Bearer "))
            {
                var token = authorization.Replace("Bearer ", "");
                var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>()
                    ?? throw new InvalidOperationException("Jwt Service Not Resolved");

                var claimsPrincipal = jwtService.Validate(token);
                if (claimsPrincipal != null)
                {
                    var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                        ?? throw new NotFoundException("User Id Claim Not Found");
                    var userId = userIdClaim.Value;
                    var userContext = context.HttpContext.RequestServices.GetService<IUserContext>()
                        ?? throw new InvalidOperationException("User Context Not Resolved");

                    userContext.UserId = Guid.Parse(userId);
                    var roleClaims = claimsPrincipal.Claims.Where(x => x.Type == ClaimTypes.Role);
                    foreach (var roleClaim in roleClaims)
                    {
                        if (_roles.Contains(roleClaim.Value))
                        {
                            isAccessGranted = true;
                            break;
                        }
                    }
                }
            }
        }

        if (_allowAnonymous)
            isAccessGranted = true;

        if (!isAccessGranted)
            UnAuthorize(context);

    }

    private static void UnAuthorize(ActionExecutingContext context)
    {
        var result = new ApiResult<string>(false, HttpStatusCode.Unauthorized, string.Empty, "دسترسی مجاز نیست");
        context.Result = new JsonResult(result) { StatusCode = (int)HttpStatusCode.Unauthorized };
    }
}
