using Entities.Users;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts.Authentications;
using Services.DTOs.Accounts.Authorization;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.Admin.v1;

[ApiVersion("1")]
public class AuthorizationAdminController(
    IAuthorizationManagementService authorizationManagementService,
    IUserContext userContext) : BaseAdminApiController
{
    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin)]
    [ApiPermissionAuthorize("admin.authz.manage")]
    public async Task UpsertRolePermissionsAsync(UpsertRolePermissionsRequest request, CancellationToken cancellationToken)
        => await authorizationManagementService.UpsertRolePermissionsAsync(request, userContext.UserId, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin)]
    [ApiPermissionAuthorize("admin.authz.manage")]
    public async Task UpsertPlanPermissionsAsync(UpsertPlanPermissionsRequest request, CancellationToken cancellationToken)
        => await authorizationManagementService.UpsertPlanPermissionsAsync(request, userContext.UserId, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin)]
    [ApiPermissionAuthorize("admin.authz.manage")]
    public async Task UpsertUserPermissionGrantAsync(UpsertUserPermissionGrantRequest request, CancellationToken cancellationToken)
        => await authorizationManagementService.UpsertUserPermissionGrantAsync(request, userContext.UserId, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin)]
    [ApiPermissionAuthorize("admin.authz.manage")]
    public async Task UpsertUserPermissionRevokeAsync(UpsertUserPermissionRevokeRequest request, CancellationToken cancellationToken)
        => await authorizationManagementService.UpsertUserPermissionRevokeAsync(request, userContext.UserId, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin)]
    [ApiPermissionAuthorize("admin.authz.manage")]
    public async Task UpsertUserPlanSubscriptionAsync(UpsertUserPlanSubscriptionRequest request, CancellationToken cancellationToken)
        => await authorizationManagementService.UpsertUserPlanSubscriptionAsync(request, userContext.UserId, cancellationToken);
}
