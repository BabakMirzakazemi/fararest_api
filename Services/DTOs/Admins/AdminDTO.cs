
namespace Services.DTOs.Admins;

public class AdminDTO 
{
    public Guid Id { get; set; }

    public string? FullName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime CodeExpirationDate { get; set; }

    public string? ConfirmationCode { get; set; }

    public List<string> Roles { get; set; } = [];

    public List<Guid> RoleIds { get; set; } = [];


}
