namespace Services.DTOs.Accounts.CompleteRegistration;
public record CompleteUserRegistrationRequest
{
    public Guid UserId { get; set; }

    public Guid AccountId { get; set; }

    public string Password { get; set; } = null!;

    public string RepeatPassword { get; set; } = null!;

}
