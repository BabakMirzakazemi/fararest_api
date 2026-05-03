using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Accounts;

public class AccountMembershipPermission : BaseEntity<long>, IEntity
{
    public long MembershipId { get; set; }
    public int PermissionId { get; set; }
    public bool IsGranted { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
}

public class AccountMembershipPermissionConfiguration : IEntityTypeConfiguration<AccountMembershipPermission>
{
    public void Configure(EntityTypeBuilder<AccountMembershipPermission> builder)
    {
        builder.ToTable("accounts_membership_permissions");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.MembershipId).HasColumnName("membership_id");
        builder.Property(x => x.PermissionId).HasColumnName("permission_id");
        builder.Property(x => x.IsGranted).HasColumnName("is_granted").HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
    }
}
