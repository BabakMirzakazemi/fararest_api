using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Accounts;

public class AccountMembership : BaseEntity<long>, IEntity
{
    public long OrganizationId { get; set; }
    public int UserId { get; set; }
    public int? PrimaryGroupId { get; set; }
    public bool IsOwner { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AccountMembershipConfiguration : IEntityTypeConfiguration<AccountMembership>
{
    public void Configure(EntityTypeBuilder<AccountMembership> builder)
    {
        builder.ToTable("accounts_membership");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.PrimaryGroupId).HasColumnName("primary_group_id");
        builder.Property(x => x.IsOwner).HasColumnName("is_owner");
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
    }
}
