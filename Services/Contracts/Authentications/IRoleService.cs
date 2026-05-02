using Services.Contracts.Repositories;
using Services.DTOs.Admins;

namespace Services.Contracts.Authentications;

public interface IRoleService : IRepository<Role>
{
    //Task<List<RoleViewDTO>> GetAllAsync(CancellationToken cancellationToken);

    //Task<List<RoleViewDTO>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken);

    //Task<OperationResult> SetRolesToAdminAsync(SetRolesAdminDTO dto, CancellationToken cancellationToken);

    Task AddRolesAsync(Guid userId, List<string> names, CancellationToken cancellationToken);
}
