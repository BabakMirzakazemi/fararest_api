using Common.Markers;

namespace Services.DTOs.Admins;

public class RoleViewDTO : IHaveCustomMapping
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public void CreateMappings(Profile profile)
    {
        profile.CreateMap<Role, RoleViewDTO>();
    }
}
