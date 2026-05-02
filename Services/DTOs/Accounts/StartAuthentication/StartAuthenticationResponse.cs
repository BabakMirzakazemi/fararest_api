namespace Services.DTOs.Accounts.StartAuthentication;

public sealed record StartAuthenticationResponse(Guid UserId, bool HasAccount);
