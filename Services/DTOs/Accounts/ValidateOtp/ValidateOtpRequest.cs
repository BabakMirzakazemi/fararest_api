namespace Services.DTOs.Accounts.ValidateOtp;
public sealed record ValidateOtpRequest
{
    public Guid UserId { get; set; }

    public string Otp { get; set; } = null!;
}
