namespace Services.DTOs.Accounts.Login;

public sealed record LoginResponse(string Token, string? Email = null, string? Mobile = null, string? FullName = null);
