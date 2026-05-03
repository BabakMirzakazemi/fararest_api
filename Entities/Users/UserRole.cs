using Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace Entities.Users;

public class UserRole : IdentityUserRole<Guid>, IEntity
{
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
