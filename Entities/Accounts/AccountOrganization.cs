using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Accounts;

public class AccountOrganization : BaseEntity<long>, IEntity
{
    public string Name { get; set; } = string.Empty;
    public int? OwnerUserId { get; set; }
    public string? TaxId { get; set; }
    public string? NationalId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int? ProvinceId { get; set; }
    public int? CountyId { get; set; }
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public string? LogoUrl { get; set; }
}

public class AccountOrganizationConfiguration : IEntityTypeConfiguration<AccountOrganization>
{
    public void Configure(EntityTypeBuilder<AccountOrganization> builder)
    {
        builder.ToTable("accounts_organization");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.OwnerUserId).HasColumnName("owner_user_id");
        builder.Property(x => x.TaxId).HasColumnName("tax_id").HasMaxLength(50);
        builder.Property(x => x.NationalId).HasColumnName("national_id").HasMaxLength(50);
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.ProvinceId).HasColumnName("province_id");
        builder.Property(x => x.CountyId).HasColumnName("county_id");
        builder.Property(x => x.Address).HasColumnName("address");
        builder.Property(x => x.PostalCode).HasColumnName("postal_code").HasMaxLength(20);
        builder.Property(x => x.LogoUrl).HasColumnName("logo_url");
    }
}
