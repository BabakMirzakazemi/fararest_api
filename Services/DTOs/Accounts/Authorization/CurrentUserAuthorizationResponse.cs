namespace Services.DTOs.Accounts.Authorization;

public sealed record CurrentUserAuthorizationResponse(
    Guid UserId,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions,
    string PermissionVersion);
