using Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace Entities.Users
{
    public class Role : IdentityRole<Guid>, IEntity
    {
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
