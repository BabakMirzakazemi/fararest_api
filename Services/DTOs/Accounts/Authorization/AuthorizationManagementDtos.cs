namespace Services.DTOs.Accounts.Authorization;

public sealed record UpsertRolePermissionsRequest(string RoleName, IReadOnlyList<string> PermissionKeys);
public sealed record UpsertPlanPermissionsRequest(string PlanCode, IReadOnlyList<string> PermissionKeys);
public sealed record UpsertUserPermissionGrantRequest(Guid UserId, string PermissionKey, string Source, string? Notes);
public sealed record UpsertUserPermissionRevokeRequest(Guid UserId, string PermissionKey, string? Notes);
public sealed record UpsertUserPlanSubscriptionRequest(Guid UserId, string PlanCode, DateTimeOffset StartsAt, DateTimeOffset? EndsAt, bool IsActive);
