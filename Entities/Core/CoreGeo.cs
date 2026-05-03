using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Core;

public class CoreProvince : BaseEntity<int>, IEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? TelPrefix { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CoreCounty : BaseEntity<int>, IEntity
{
    public int ProvinceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CoreProvinceConfiguration : IEntityTypeConfiguration<CoreProvince>
{
    public void Configure(EntityTypeBuilder<CoreProvince> builder)
    {
        builder.ToTable("core_province");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(120);
        builder.Property(x => x.TelPrefix).HasColumnName("tel_prefix").HasMaxLength(8);
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.Slug).IsUnique().HasFilter("(slug IS NOT NULL)");
    }
}

public class CoreCountyConfiguration : IEntityTypeConfiguration<CoreCounty>
{
    public void Configure(EntityTypeBuilder<CoreCounty> builder)
    {
        builder.ToTable("core_county");
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.ProvinceId).HasColumnName("province_id");
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(120).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(140);
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

        builder.HasIndex(x => x.ProvinceId);
        builder.HasIndex(x => new { x.ProvinceId, x.Name }).IsUnique();
        builder.HasIndex(x => new { x.ProvinceId, x.Slug }).IsUnique().HasFilter("(slug IS NOT NULL)");
    }
}
