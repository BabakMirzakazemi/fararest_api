namespace Services.DTOs.Accounts.Mfa;

public sealed record MfaStatusResponse(bool IsEnabled, bool IsRequiredByRole);

public sealed record SetMfaStatusRequest
{
    public bool IsEnabled { get; set; }
    public required string Otp { get; set; }
}

