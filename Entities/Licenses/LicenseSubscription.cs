using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Licenses;

public class LicenseSubscription : BaseEntity<long>, IEntity
{
    public long OrganizationId { get; set; }
    public long PlanId { get; set; }
    public long BillingCycleId { get; set; }
    public long? QueuedFromSubscriptionId { get; set; }
    public DateTimeOffset? RequestedStartAt { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal PriceAmount { get; set; }
    public string CurrencyCode { get; set; } = "IRR";
    public DateTimeOffset PurchasedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class LicenseSubscriptionConfiguration : IEntityTypeConfiguration<LicenseSubscription>
{
    public void Configure(EntityTypeBuilder<LicenseSubscription> builder)
    {
        builder.ToTable("licenses_subscription");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id");
        builder.Property(x => x.PlanId).HasColumnName("plan_id");
        builder.Property(x => x.BillingCycleId).HasColumnName("billing_cycle_id");
        builder.Property(x => x.QueuedFromSubscriptionId).HasColumnName("queued_from_subscription_id");
        builder.Property(x => x.RequestedStartAt).HasColumnName("requested_start_at");
        builder.Property(x => x.StartsAt).HasColumnName("starts_at");
        builder.Property(x => x.EndsAt).HasColumnName("ends_at");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(x => x.PriceAmount).HasColumnName("price_amount").HasColumnType("numeric(18,2)");
        builder.Property(x => x.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3).IsFixedLength();
        builder.Property(x => x.PurchasedAt).HasColumnName("purchased_at");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
    }
}
