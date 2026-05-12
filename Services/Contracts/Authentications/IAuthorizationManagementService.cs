using Services.DTOs.Accounts.Authorization;

namespace Services.Contracts.Authentications;

public interface IAuthorizationManagementService
{
    Task UpsertRolePermissionsAsync(UpsertRolePermissionsRequest request, Guid actorUserId, CancellationToken cancellationToken);
    Task UpsertPlanPermissionsAsync(UpsertPlanPermissionsRequest request, Guid actorUserId, CancellationToken cancellationToken);
    Task UpsertUserPermissionGrantAsync(UpsertUserPermissionGrantRequest request, Guid actorUserId, CancellationToken cancellationToken);
    Task UpsertUserPermissionRevokeAsync(UpsertUserPermissionRevokeRequest request, Guid actorUserId, CancellationToken cancellationToken);
    Task UpsertUserPlanSubscriptionAsync(UpsertUserPlanSubscriptionRequest request, Guid actorUserId, CancellationToken cancellationToken);
}
