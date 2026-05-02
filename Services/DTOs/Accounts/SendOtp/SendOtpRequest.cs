namespace Services.DTOs.Accounts.SendOtp;

public sealed record SendOtpRequest
{
    public Guid UserId { get; set; }

    public Guid AccountId { get; set; }
}
