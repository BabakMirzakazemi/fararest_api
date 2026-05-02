namespace Services.DTOs.Accounts.Login;

public sealed record SendPhoneLoginOtpRequest
{
    public string Mobile { get; set; } = null!;
}
