namespace Services.DTOs.Accounts.Login;

public sealed record EmailPasswordLoginRequest
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}
