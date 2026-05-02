namespace Services.DTOs.Accounts.Registration;

public sealed record RegisterWithEmailRequest
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string ConfirmPassword { get; set; } = null!;

    public string? FullName { get; set; }
}
