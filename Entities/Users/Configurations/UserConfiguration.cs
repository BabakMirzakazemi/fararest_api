using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Users.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(p => p.UserName).IsRequired().HasMaxLength(100);
        builder.HasIndex(p => p.UserName).IsUnique();
        // TODO(QueryTuning): after heavy business queries are finalized, add targeted indexes
        // based on real execution plans (for example filters/sorts used by user listing endpoints).
        builder.Property(p => p.IsActive).HasDefaultValue(true);
        builder.Ignore(u => u.PhoneNumber);
        builder.Navigation(u => u.ConfirmationCode).AutoInclude();
        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
