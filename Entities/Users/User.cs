using Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace Entities.Users
{
    public class User : IdentityUser<Guid>, IEntity
    {
        public User()
        {
            IsActive = true;
        }

        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string? AvatarPath { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? NationalCode { get; set; }
        public string? FullName { get; set; }
        public string? Mobile { get; set; }
        public int ConfirmationCodeId { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ConfirmationCode ConfirmationCode { get; set; } = null!;
    }
}
