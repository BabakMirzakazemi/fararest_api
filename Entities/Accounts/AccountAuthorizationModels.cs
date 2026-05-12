using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Accounts;

public class AccountPermission : BaseEntity<long>, IEntity
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class AccountRolePermission : BaseEntity<long>, IEntity
{
    public Guid RoleId { get; set; }
    public long PermissionId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
}

public class AccountPlanPermission : BaseEntity<long>, IEntity
{
    public long PlanId { get; set; }
    public long PermissionId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
}

public class AccountUserPermissionGrant : BaseEntity<long>, IEntity
{
    public Guid UserId { get; set; }
    public long PermissionId { get; set; }
    public string Source { get; set; } = "manual";
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
}

public class AccountUserPermissionRevoke : BaseEntity<long>, IEntity
{
    public Guid UserId { get; set; }
    public long PermissionId { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
}

public class AccountUserPlanSubscription : BaseEntity<long>, IEntity
{
    public Guid UserId { get; set; }
    public long PlanId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
}

public class AccountPermissionConfiguration : IEntityTypeConfiguration<AccountPermission>
{
    public void Configure(EntityTypeBuilder<AccountPermission> builder)
    {
        builder.ToTable("accounts_permission");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Key).HasColumnName("key").HasMaxLength(120).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Category).HasColumnName("category").HasMaxLength(80).IsRequired();
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.HasIndex(x => x.Key).IsUnique();
    }
}

public class AccountRolePermissionConfiguration : IEntityTypeConfiguration<AccountRolePermission>
{
    public void Configure(EntityTypeBuilder<AccountRolePermission> builder)
    {
        builder.ToTable("accounts_role_permission");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.RoleId).HasColumnName("role_id");
        builder.Property(x => x.PermissionId).HasColumnName("permission_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id");
        builder.HasIndex(x => new { x.RoleId, x.PermissionId }).IsUnique();
    }
}

public class AccountPlanPermissionConfiguration : IEntityTypeConfiguration<AccountPlanPermission>
{
    public void Configure(EntityTypeBuilder<AccountPlanPermission> builder)
    {
        builder.ToTable("accounts_plan_permission");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PlanId).HasColumnName("plan_id");
        builder.Property(x => x.PermissionId).HasColumnName("permission_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id");
        builder.HasIndex(x => new { x.PlanId, x.PermissionId }).IsUnique();
    }
}

public class AccountUserPermissionGrantConfiguration : IEntityTypeConfiguration<AccountUserPermissionGrant>
{
    public void Configure(EntityTypeBuilder<AccountUserPermissionGrant> builder)
    {
        builder.ToTable("accounts_user_permission_grant");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.PermissionId).HasColumnName("permission_id");
        builder.Property(x => x.Source).HasColumnName("source").HasMaxLength(40).IsRequired();
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(500);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id");
        builder.HasIndex(x => new { x.UserId, x.PermissionId }).IsUnique();
    }
}

public class AccountUserPermissionRevokeConfiguration : IEntityTypeConfiguration<AccountUserPermissionRevoke>
{
    public void Configure(EntityTypeBuilder<AccountUserPermissionRevoke> builder)
    {
        builder.ToTable("accounts_user_permission_revoke");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.PermissionId).HasColumnName("permission_id");
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(500);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id");
        builder.HasIndex(x => new { x.UserId, x.PermissionId }).IsUnique();
    }
}

public class AccountUserPlanSubscriptionConfiguration : IEntityTypeConfiguration<AccountUserPlanSubscription>
{
    public void Configure(EntityTypeBuilder<AccountUserPlanSubscription> builder)
    {
        builder.ToTable("accounts_user_plan_subscription");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.PlanId).HasColumnName("plan_id");
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(x => x.StartsAt).HasColumnName("starts_at");
        builder.Property(x => x.EndsAt).HasColumnName("ends_at");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id");
        builder.HasIndex(x => new { x.UserId, x.PlanId, x.IsActive });
    }
}
