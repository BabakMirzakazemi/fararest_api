namespace Services.DTOs.Accounts.Registration;

public sealed record ResendEmailActivationLinkRequest
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = null!;
}
