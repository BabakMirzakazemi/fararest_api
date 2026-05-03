using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Licenses;

public class LicensePlanPrice : BaseEntity<long>, IEntity
{
    public long PlanId { get; set; }
    public long BillingCycleId { get; set; }
    public decimal PriceAmount { get; set; }
    public string CurrencyCode { get; set; } = "IRR";
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public decimal DiscountAmount { get; set; }
}

public class LicensePlanPriceConfiguration : IEntityTypeConfiguration<LicensePlanPrice>
{
    public void Configure(EntityTypeBuilder<LicensePlanPrice> builder)
    {
        builder.ToTable("licenses_plan_price");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PlanId).HasColumnName("plan_id");
        builder.Property(x => x.BillingCycleId).HasColumnName("billing_cycle_id");
        builder.Property(x => x.PriceAmount).HasColumnName("price_amount").HasColumnType("numeric(18,2)");
        builder.Property(x => x.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3).IsFixedLength();
        builder.Property(x => x.IsActive).HasColumnName("is_active");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DiscountAmount).HasColumnName("discount_amount").HasColumnType("numeric(18,2)");
    }
}
