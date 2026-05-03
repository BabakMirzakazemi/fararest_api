namespace Services.DTOs.Accounts.Recovery;

public sealed record ForgotPasswordRequest
{
    public string? Email { get; set; }
    public string? Mobile { get; set; }
}

