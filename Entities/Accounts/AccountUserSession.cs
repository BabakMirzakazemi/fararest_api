using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Net;

namespace Entities.Accounts;

public class AccountUserSession : BaseEntity<long>, IEntity
{
    public Guid SessionPublicId { get; set; }
    public int UserId { get; set; }
    public long? OrganizationId { get; set; }
    public string SessionSecretHash { get; set; } = string.Empty;
    public string AuthMethod { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public string? OsName { get; set; }
    public string? BrowserName { get; set; }
    public IPAddress? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTimeOffset IssuedAt { get; set; }
    public DateTimeOffset LastSeenAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? RevokeReason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AccountUserSessionConfiguration : IEntityTypeConfiguration<AccountUserSession>
{
    public void Configure(EntityTypeBuilder<AccountUserSession> builder)
    {
        builder.ToTable("accounts_user_session");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.SessionPublicId).HasColumnName("session_public_id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id");
        builder.Property(x => x.SessionSecretHash).HasColumnName("session_secret_hash").HasColumnType("char(64)");
        builder.Property(x => x.AuthMethod).HasColumnName("auth_method").HasMaxLength(30).IsRequired();
        builder.Property(x => x.DeviceType).HasColumnName("device_type").HasMaxLength(30).IsRequired();
        builder.Property(x => x.DeviceName).HasColumnName("device_name").HasMaxLength(120);
        builder.Property(x => x.OsName).HasColumnName("os_name").HasMaxLength(120);
        builder.Property(x => x.BrowserName).HasColumnName("browser_name").HasMaxLength(120);
        builder.Property(x => x.IpAddress).HasColumnName("ip_address").HasColumnType("inet");
        builder.Property(x => x.UserAgent).HasColumnName("user_agent");
        builder.Property(x => x.IssuedAt).HasColumnName("issued_at");
        builder.Property(x => x.LastSeenAt).HasColumnName("last_seen_at");
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at");
        builder.Property(x => x.RevokedAt).HasColumnName("revoked_at");
        builder.Property(x => x.RevokeReason).HasColumnName("revoke_reason").HasMaxLength(120);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
    }
}
