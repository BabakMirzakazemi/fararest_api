namespace Services.DTOs.Accounts.Sessions;

public sealed record AccountSessionItemDto(
    Guid SessionPublicId,
    string AuthMethod,
    string DeviceType,
    string? DeviceName,
    string? OsName,
    string? BrowserName,
    string? IpAddress,
    DateTimeOffset IssuedAt,
    DateTimeOffset LastSeenAt,
    DateTimeOffset ExpiresAt,
    bool IsCurrentSession);

public sealed record RevokeSessionRequest
{
    public Guid SessionPublicId { get; set; }
};

