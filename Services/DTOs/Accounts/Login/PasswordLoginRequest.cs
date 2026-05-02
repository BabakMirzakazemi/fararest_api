namespace Services.DTOs.Accounts.Login;

public sealed record PasswordLoginRequest( Guid UserId, string Password);
