using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Licenses;

public class LicensePlanModule : BaseEntity<long>, IEntity
{
    public long PlanId { get; set; }
    public long ModuleId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
}

public class LicensePlanModuleConfiguration : IEntityTypeConfiguration<LicensePlanModule>
{
    public void Configure(EntityTypeBuilder<LicensePlanModule> builder)
    {
        builder.ToTable("licenses_plan_module");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PlanId).HasColumnName("plan_id");
        builder.Property(x => x.ModuleId).HasColumnName("module_id");
        builder.Property(x => x.IsEnabled).HasColumnName("is_enabled");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
    }
}
