namespace Services.DTOs.Accounts.Login;

public sealed record PhoneOtpLoginRequest
{
    public string Mobile { get; set; } = null!;

    public string Otp { get; set; } = null!;
}
