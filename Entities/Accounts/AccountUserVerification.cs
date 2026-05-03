using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Accounts;

public class AccountUserVerification : BaseEntity<long>, IEntity
{
    public int UserId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string OtpCodeHash { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public short AttemptCount { get; set; }
    public short MaxAttempts { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? VerifiedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AccountUserVerificationConfiguration : IEntityTypeConfiguration<AccountUserVerification>
{
    public void Configure(EntityTypeBuilder<AccountUserVerification> builder)
    {
        builder.ToTable("accounts_user_verification");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Channel).HasColumnName("channel").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Purpose).HasColumnName("purpose").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Destination).HasColumnName("destination").HasMaxLength(255).IsRequired();
        builder.Property(x => x.OtpCodeHash).HasColumnName("otp_code_hash").HasColumnType("char(64)");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(x => x.AttemptCount).HasColumnName("attempt_count");
        builder.Property(x => x.MaxAttempts).HasColumnName("max_attempts");
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at");
        builder.Property(x => x.VerifiedAt).HasColumnName("verified_at");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
    }
}
