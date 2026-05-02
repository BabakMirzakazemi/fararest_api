namespace Services.DTOs.Users.UpdatePassword;

public sealed record UpdatePasswordRequest
{
    public required string CurrentPassword { get; set; }

    public required string NewPassword { get; set; }

    public required string ConfirmNewPassword { get; set; }

    public required string Otp { get; set; }
}
