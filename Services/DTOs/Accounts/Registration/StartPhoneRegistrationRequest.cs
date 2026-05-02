namespace Services.DTOs.Accounts.Registration;

public sealed record StartPhoneRegistrationRequest
{
    public string Mobile { get; set; } = null!;

    public string? FullName { get; set; }
}
