namespace Services.DTOs.Accounts.Registration;

public sealed record CompletePhoneRegistrationRequest
{
    public Guid UserId { get; set; }

    public string Mobile { get; set; } = null!;

    public string Otp { get; set; } = null!;
}
