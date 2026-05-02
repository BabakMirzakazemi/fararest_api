namespace Services.DTOs.Accounts.Registration;

public sealed record ConfirmEmailRegistrationRequest
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = null!;

    public string Otp { get; set; } = null!;
}
