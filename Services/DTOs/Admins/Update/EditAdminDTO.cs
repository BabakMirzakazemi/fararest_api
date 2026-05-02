namespace Services.DTOs.Admins.Update;

public class EditAdminDTO
{
    public Guid Id { get; set; }
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
