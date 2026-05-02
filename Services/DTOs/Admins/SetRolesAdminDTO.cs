namespace Services.DTOs.Admins;

public class SetRolesAdminDTO
{
    public Guid UserId { get; set; }
    public List<string> RoleIds { get; set; } = [];
}
