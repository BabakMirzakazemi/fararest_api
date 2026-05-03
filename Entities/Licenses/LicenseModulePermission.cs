using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Licenses;

public class LicenseModulePermission : BaseEntity<long>, IEntity
{
    public long ModuleId { get; set; }
    public int PermissionId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class LicenseModulePermissionConfiguration : IEntityTypeConfiguration<LicenseModulePermission>
{
    public void Configure(EntityTypeBuilder<LicenseModulePermission> builder)
    {
        builder.ToTable("licenses_module_permission");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ModuleId).HasColumnName("module_id");
        builder.Property(x => x.PermissionId).HasColumnName("permission_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
    }
}
