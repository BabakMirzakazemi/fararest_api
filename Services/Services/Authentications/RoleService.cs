using Common.Markers;
using Services.Contracts.Authentications;
using Services.Contracts.Repositories;
using Services.DTOs.Admins;
using Services.Services.Repositories;

namespace Services.Services.Authentications;

public class RoleService : Repository<Role>, IRoleService, IScopedDependency
{

    private readonly IMapper _mapper;
    private readonly IRepository<UserRole> _accountRole;

    public RoleService(
                        DbContext dbContext,
                        IMapper mapper,
                        IRepository<UserRole> accountRole
                        ) : base(dbContext)
    {
        _mapper = mapper;
        _accountRole = accountRole;
    }

    //TODO Review After New User Management Flow
    public async Task<List<RoleViewDTO>> GetAllAsync(CancellationToken cancellationToken) =>

        await TableNoTracking.Select(x => x).ProjectTo<RoleViewDTO>(_mapper.ConfigurationProvider)
                             .ToListAsync(cancellationToken);




    //TODO Review After New User Management Flow
    public async Task<List<RoleViewDTO>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken) =>
        await _accountRole.TableNoTracking.Where(x => x.UserId == userId)
                    .Select(x => x.Role).ProjectTo<RoleViewDTO>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);



    //TODO Review After New User Management Flow
    public async Task<OperationResult> SetRolesToAdminAsync(SetRolesAdminDTO dto, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return new OperationResult().SetProperties(false, "SetRolesToAdminAsync is not implemented yet.");

        //OperationResultDTO result = new OperationResultDTO();
        //User? user = await _userService.GetByIdAsync(cancellationToken, dto.UserId);
        //if (user == null)
        //    result.SetProperties(false, ApplicationMessages.NotFoundUserOrCompany);
        //else
        //{
        //    var userRoles = await _userManager.GetRolesAsync(user);
        //    await _userManager.RemoveFromRolesAsync(user, userRoles.ToArray());
        //    List<Role> roles = new List<Role>();
        //    foreach (var item in dto.RoleIds)
        //    {
        //        Role? role = await TableNoTracking.Where(x => x.UserId.ToString() == item).FirstOrDefaultAsync(cancellationToken);
        //        if (role != null)
        //            roles.Add(role);
        //    }
        //    await _userManager.AddToRolesAsync(user, roles.Select(x => x.Name));
        //
        //    result.SetProperties(true, ApplicationMessages.SuccessOperation);
        //}
        //
        //return result;
    }

    public async Task AddRolesAsync(Guid userId, List<string> names, CancellationToken cancellationToken)
    {
        var accountRoles = await _accountRole
            .TableNoTracking
            .Where(ar => ar.User.Id == userId)
            .ToListAsync(cancellationToken);

        //TODO Review After New User Management Flow
        var newAccountRoles = new List<UserRole>();
        await _accountRole.UpdateRangeAsync(accountRoles, cancellationToken);
        await _accountRole.AddRangeAsync(newAccountRoles, cancellationToken);
    }
}
