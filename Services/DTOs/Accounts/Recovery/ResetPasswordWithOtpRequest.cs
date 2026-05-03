namespace Services.DTOs.Accounts.Recovery;

public sealed record ResetPasswordWithOtpRequest
{
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public required string Otp { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmNewPassword { get; set; }
}

