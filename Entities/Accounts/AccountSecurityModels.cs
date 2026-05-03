using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Accounts;

public class AccountSecurityFeature : BaseEntity<long>, IEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool DefaultEnabled { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AccountOrganizationSecuritySetting : BaseEntity<long>, IEntity
{
    public long OrganizationId { get; set; }
    public long FeatureId { get; set; }
    public bool IsEnabled { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AccountUserSecuritySetting : BaseEntity<long>, IEntity
{
    public int UserId { get; set; }
    public long? OrganizationId { get; set; }
    public long FeatureId { get; set; }
    public bool IsEnabled { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AccountUserTotpFactor : BaseEntity<long>, IEntity
{
    public int UserId { get; set; }
    public long? OrganizationId { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string? AccountLabel { get; set; }
    public byte[] SecretEncrypted { get; set; } = [];
    public string Algorithm { get; set; } = string.Empty;
    public short Digits { get; set; }
    public int PeriodSeconds { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? EnabledAt { get; set; }
    public DateTimeOffset? DisabledAt { get; set; }
    public long LastUsedCounter { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AccountUserOauthIdentity : BaseEntity<long>, IEntity
{
    public int UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ProviderSubject { get; set; } = string.Empty;
    public string? ProviderEmail { get; set; }
    public bool ProviderEmailVerified { get; set; }
    public string? ProviderDisplayName { get; set; }
    public string? ProviderPictureUrl { get; set; }
    public DateTimeOffset LinkedAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AccountSecurityFeatureConfiguration : IEntityTypeConfiguration<AccountSecurityFeature>
{
    public void Configure(EntityTypeBuilder<AccountSecurityFeature> builder)
    {
        builder.ToTable("accounts_security_feature");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(80).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.DefaultEnabled).HasColumnName("default_enabled");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
    }
}

public class AccountOrganizationSecuritySettingConfiguration : IEntityTypeConfiguration<AccountOrganizationSecuritySetting>
{
    public void Configure(EntityTypeBuilder<AccountOrganizationSecuritySetting> builder)
    {
        builder.ToTable("accounts_organization_security_setting");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id");
        builder.Property(x => x.FeatureId).HasColumnName("feature_id");
        builder.Property(x => x.IsEnabled).HasColumnName("is_enabled");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
    }
}

public class AccountUserSecuritySettingConfiguration : IEntityTypeConfiguration<AccountUserSecuritySetting>
{
    public void Configure(EntityTypeBuilder<AccountUserSecuritySetting> builder)
    {
        builder.ToTable("accounts_user_security_setting");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id");
        builder.Property(x => x.FeatureId).HasColumnName("feature_id");
        builder.Property(x => x.IsEnabled).HasColumnName("is_enabled");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
    }
}

public class AccountUserTotpFactorConfiguration : IEntityTypeConfiguration<AccountUserTotpFactor>
{
    public void Configure(EntityTypeBuilder<AccountUserTotpFactor> builder)
    {
        builder.ToTable("accounts_user_totp_factor");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id");
        builder.Property(x => x.Issuer).HasColumnName("issuer").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AccountLabel).HasColumnName("account_label").HasMaxLength(255);
        builder.Property(x => x.SecretEncrypted).HasColumnName("secret_encrypted");
        builder.Property(x => x.Algorithm).HasColumnName("algorithm").HasMaxLength(30).IsRequired();
        builder.Property(x => x.Digits).HasColumnName("digits");
        builder.Property(x => x.PeriodSeconds).HasColumnName("period_seconds");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(x => x.EnabledAt).HasColumnName("enabled_at");
        builder.Property(x => x.DisabledAt).HasColumnName("disabled_at");
        builder.Property(x => x.LastUsedCounter).HasColumnName("last_used_counter");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
    }
}

public class AccountUserOauthIdentityConfiguration : IEntityTypeConfiguration<AccountUserOauthIdentity>
{
    public void Configure(EntityTypeBuilder<AccountUserOauthIdentity> builder)
    {
        builder.ToTable("accounts_user_oauth_identity");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.Provider).HasColumnName("provider").HasMaxLength(30).IsRequired();
        builder.Property(x => x.ProviderSubject).HasColumnName("provider_subject").HasMaxLength(255).IsRequired();
        builder.Property(x => x.ProviderEmail).HasColumnName("provider_email").HasMaxLength(255);
        builder.Property(x => x.ProviderEmailVerified).HasColumnName("provider_email_verified");
        builder.Property(x => x.ProviderDisplayName).HasColumnName("provider_display_name").HasMaxLength(255);
        builder.Property(x => x.ProviderPictureUrl).HasColumnName("provider_picture_url");
        builder.Property(x => x.LinkedAt).HasColumnName("linked_at");
        builder.Property(x => x.LastLoginAt).HasColumnName("last_login_at");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
    }
}
