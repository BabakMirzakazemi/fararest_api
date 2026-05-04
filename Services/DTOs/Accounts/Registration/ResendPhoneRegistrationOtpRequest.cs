namespace Services.DTOs.Accounts.Registration;

public sealed record ResendPhoneRegistrationOtpRequest
{
    public Guid UserId { get; set; }

    public string Mobile { get; set; } = null!;
}
