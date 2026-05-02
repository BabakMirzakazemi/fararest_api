using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Users.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasOne(p => p.User).WithMany(c => c.UserRoles).HasForeignKey(p => p.UserId);
        builder.HasOne(p => p.Role).WithMany(c => c.UserRoles).HasForeignKey(p => p.RoleId);
    }
}
