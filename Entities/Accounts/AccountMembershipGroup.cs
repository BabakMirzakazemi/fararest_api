using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Accounts;

public class AccountMembershipGroup : BaseEntity<long>, IEntity
{
    public long MembershipId { get; set; }
    public int GroupId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class AccountMembershipGroupConfiguration : IEntityTypeConfiguration<AccountMembershipGroup>
{
    public void Configure(EntityTypeBuilder<AccountMembershipGroup> builder)
    {
        builder.ToTable("accounts_membership_groups");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.MembershipId).HasColumnName("membership_id");
        builder.Property(x => x.GroupId).HasColumnName("group_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
    }
}
