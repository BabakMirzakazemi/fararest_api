using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Payments;

public class PaymentWallet : BaseEntity<long>, IEntity
{
    public long OrganizationId { get; set; }
    public string CurrencyCode { get; set; } = "IRR";
    public decimal BalanceAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class PaymentWalletConfiguration : IEntityTypeConfiguration<PaymentWallet>
{
    public void Configure(EntityTypeBuilder<PaymentWallet> builder)
    {
        builder.ToTable("payments_wallet");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id");
        builder.Property(x => x.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3).IsFixedLength().HasDefaultValue("IRR");
        builder.Property(x => x.BalanceAmount).HasColumnName("balance_amount").HasColumnType("numeric(16,2)").HasDefaultValue(0m);
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.HasIndex(x => x.OrganizationId).IsUnique();
    }
}
