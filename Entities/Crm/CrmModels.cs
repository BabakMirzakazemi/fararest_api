using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Crm;

public class CrmCustomer : BaseEntity<long>, IEntity
{
    public long OrganizationId { get; set; }
    public string CustomerNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public bool AllowCredit { get; set; }
    public decimal CreditLimitAmount { get; set; }
    public decimal TotalDebitAmount { get; set; }
    public decimal TotalCreditAmount { get; set; }
    public decimal NetBalanceAmount { get; set; }
    public string BalanceStatus { get; set; } = string.Empty;
    public string PaymentBehavior { get; set; } = string.Empty;
    public bool IsLoyalCustomer { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset? LastActivityAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool AllowInstallment { get; set; }
}

public class CrmCustomerLedger : BaseEntity<long>, IEntity
{
    public long CustomerId { get; set; }
    public string EntryType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public string? Note { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CrmCustomerInterest : BaseEntity<long>, IEntity
{
    public long CustomerId { get; set; }
    public long InterestTagId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CrmCustomerLoyalty : BaseEntity<long>, IEntity
{
    public long CustomerId { get; set; }
    public long? TierId { get; set; }
    public int PointsBalance { get; set; }
    public decimal TotalSpentAmount { get; set; }
    public int TotalVisitCount { get; set; }
    public DateTimeOffset? LastVisitAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CrmCustomerNote : BaseEntity<long>, IEntity
{
    public long CustomerId { get; set; }
    public string NoteType { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsImportant { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CrmInterestTag : BaseEntity<long>, IEntity
{
    public long OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CrmLoyaltyTier : BaseEntity<long>, IEntity
{
    public long OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public short RankNo { get; set; }
    public int MinPoints { get; set; }
    public decimal MinTotalSpent { get; set; }
    public string? BenefitsDescription { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CrmOrganizationPhone : BaseEntity<long>, IEntity
{
    public long OrganizationId { get; set; }
    public string Phone { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CrmDiscountCampaign : BaseEntity<long>, IEntity
{
    public long OrganizationId { get; set; }
    public string CampaignType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public int? UsageLimitTotal { get; set; }
    public int? UsageLimitPerCustomer { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CrmDiscountCampaignItemRule : BaseEntity<long>, IEntity
{
    public long CampaignId { get; set; }
    public string RuleRole { get; set; } = string.Empty;
    public long ItemId { get; set; }
    public decimal MinQty { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CrmDiscountCampaignTargetCustomer : BaseEntity<long>, IEntity
{
    public long CampaignId { get; set; }
    public long CustomerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CrmDiscountCampaignUsage : BaseEntity<long>, IEntity
{
    public long CampaignId { get; set; }
    public long? CustomerId { get; set; }
    public string? OrderRef { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CrmDiscountCoupon : BaseEntity<long>, IEntity
{
    public long CampaignId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class CrmCustomerFinancialSummary : IEntity
{
    public long? CustomerId { get; set; }
    public long? OrganizationId { get; set; }
    public string? CustomerNo { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? BalanceStatus { get; set; }
    public decimal? TotalDebitAmount { get; set; }
    public decimal? TotalCreditAmount { get; set; }
    public decimal? NetBalanceAmount { get; set; }
    public bool? AllowCredit { get; set; }
    public decimal? CreditLimitAmount { get; set; }
}

public class CrmCommonConfiguration :
    IEntityTypeConfiguration<CrmCustomer>, IEntityTypeConfiguration<CrmCustomerLedger>,
    IEntityTypeConfiguration<CrmCustomerInterest>, IEntityTypeConfiguration<CrmCustomerLoyalty>,
    IEntityTypeConfiguration<CrmCustomerNote>, IEntityTypeConfiguration<CrmInterestTag>,
    IEntityTypeConfiguration<CrmLoyaltyTier>, IEntityTypeConfiguration<CrmOrganizationPhone>,
    IEntityTypeConfiguration<CrmDiscountCampaign>, IEntityTypeConfiguration<CrmDiscountCampaignItemRule>,
    IEntityTypeConfiguration<CrmDiscountCampaignTargetCustomer>, IEntityTypeConfiguration<CrmDiscountCampaignUsage>,
    IEntityTypeConfiguration<CrmDiscountCoupon>, IEntityTypeConfiguration<CrmCustomerFinancialSummary>
{
    public void Configure(EntityTypeBuilder<CrmCustomer> b)
    {
        b.ToTable("crm_customer");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.OrganizationId).HasColumnName("organization_id");
        b.Property(x => x.CustomerNo).HasColumnName("customer_no").HasMaxLength(24);
        b.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(160);
        b.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20);
        b.Property(x => x.Email).HasColumnName("email").HasMaxLength(254);
        b.Property(x => x.Address).HasColumnName("address");
        b.Property(x => x.BirthDate).HasColumnName("birth_date");
        b.Property(x => x.AllowCredit).HasColumnName("allow_credit");
        b.Property(x => x.CreditLimitAmount).HasColumnName("credit_limit_amount").HasPrecision(14, 2);
        b.Property(x => x.TotalDebitAmount).HasColumnName("total_debit_amount").HasPrecision(14, 2);
        b.Property(x => x.TotalCreditAmount).HasColumnName("total_credit_amount").HasPrecision(14, 2);
        b.Property(x => x.NetBalanceAmount).HasColumnName("net_balance_amount").HasPrecision(14, 2);
        b.Property(x => x.BalanceStatus).HasColumnName("balance_status").HasMaxLength(10).HasDefaultValueSql("'settled'::character varying");
        b.Property(x => x.PaymentBehavior).HasColumnName("payment_behavior").HasMaxLength(10).HasDefaultValueSql("'neutral'::character varying");
        b.Property(x => x.IsLoyalCustomer).HasColumnName("is_loyal_customer");
        b.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        b.Property(x => x.LastActivityAt).HasColumnName("last_activity_at");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
        b.Property(x => x.AllowInstallment).HasColumnName("allow_installment");

        b.HasIndex(x => new { x.OrganizationId, x.CustomerNo }).IsUnique();
        b.HasIndex(x => new { x.OrganizationId, x.Phone }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<CrmCustomerLedger> b)
    {
        b.ToTable("crm_customer_ledger");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.CustomerId).HasColumnName("customer_id");
        b.Property(x => x.EntryType).HasColumnName("entry_type").HasMaxLength(10);
        b.Property(x => x.Amount).HasColumnName("amount").HasPrecision(14, 2);
        b.Property(x => x.DueAt).HasColumnName("due_at");
        b.Property(x => x.Note).HasColumnName("note");
        b.Property(x => x.ReferenceType).HasColumnName("reference_type").HasMaxLength(30);
        b.Property(x => x.ReferenceId).HasColumnName("reference_id").HasMaxLength(50);
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.CustomerId, x.CreatedAt });
    }

    public void Configure(EntityTypeBuilder<CrmCustomerInterest> b)
    {
        b.ToTable("crm_customer_interest");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.CustomerId).HasColumnName("customer_id");
        b.Property(x => x.InterestTagId).HasColumnName("interest_tag_id");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => x.CustomerId);
        b.HasIndex(x => x.InterestTagId);
        b.HasIndex(x => new { x.CustomerId, x.InterestTagId }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<CrmCustomerLoyalty> b)
    {
        b.ToTable("crm_customer_loyalty");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.CustomerId).HasColumnName("customer_id");
        b.Property(x => x.TierId).HasColumnName("tier_id");
        b.Property(x => x.PointsBalance).HasColumnName("points_balance");
        b.Property(x => x.TotalSpentAmount).HasColumnName("total_spent_amount").HasPrecision(14, 2);
        b.Property(x => x.TotalVisitCount).HasColumnName("total_visit_count");
        b.Property(x => x.LastVisitAt).HasColumnName("last_visit_at");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
        b.HasIndex(x => x.CustomerId).IsUnique();
    }

    public void Configure(EntityTypeBuilder<CrmCustomerNote> b)
    {
        b.ToTable("crm_customer_note");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.CustomerId).HasColumnName("customer_id");
        b.Property(x => x.NoteType).HasColumnName("note_type").HasMaxLength(20).HasDefaultValueSql("'general'::character varying");
        b.Property(x => x.Body).HasColumnName("body");
        b.Property(x => x.IsImportant).HasColumnName("is_important");
        b.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.CustomerId, x.CreatedAt });
    }

    public void Configure(EntityTypeBuilder<CrmInterestTag> b)
    {
        b.ToTable("crm_interest_tag");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.OrganizationId).HasColumnName("organization_id");
        b.Property(x => x.Name).HasColumnName("name").HasMaxLength(80);
        b.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.OrganizationId, x.Name }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<CrmLoyaltyTier> b)
    {
        b.ToTable("crm_loyalty_tier");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.OrganizationId).HasColumnName("organization_id");
        b.Property(x => x.Name).HasColumnName("name").HasMaxLength(60);
        b.Property(x => x.RankNo).HasColumnName("rank_no");
        b.Property(x => x.MinPoints).HasColumnName("min_points");
        b.Property(x => x.MinTotalSpent).HasColumnName("min_total_spent").HasPrecision(14, 2);
        b.Property(x => x.BenefitsDescription).HasColumnName("benefits_description");
        b.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.OrganizationId, x.Name }).IsUnique();
        b.HasIndex(x => new { x.OrganizationId, x.RankNo }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<CrmOrganizationPhone> b)
    {
        b.ToTable("crm_organization_phone");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.OrganizationId).HasColumnName("organization_id");
        b.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20);
        b.Property(x => x.IsPrimary).HasColumnName("is_primary");
        b.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.OrganizationId, x.Phone }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<CrmDiscountCampaign> b)
    {
        b.ToTable("crm_discount_campaign");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.OrganizationId).HasColumnName("organization_id");
        b.Property(x => x.CampaignType).HasColumnName("campaign_type").HasMaxLength(20);
        b.Property(x => x.Title).HasColumnName("title").HasMaxLength(160);
        b.Property(x => x.Description).HasColumnName("description");
        b.Property(x => x.StartsAt).HasColumnName("starts_at");
        b.Property(x => x.EndsAt).HasColumnName("ends_at");
        b.Property(x => x.DiscountType).HasColumnName("discount_type").HasMaxLength(10).HasDefaultValueSql("'percent'::character varying");
        b.Property(x => x.DiscountValue).HasColumnName("discount_value").HasPrecision(12, 2);
        b.Property(x => x.MinOrderAmount).HasColumnName("min_order_amount").HasPrecision(14, 2);
        b.Property(x => x.MaxDiscountAmount).HasColumnName("max_discount_amount").HasPrecision(14, 2);
        b.Property(x => x.UsageLimitTotal).HasColumnName("usage_limit_total");
        b.Property(x => x.UsageLimitPerCustomer).HasColumnName("usage_limit_per_customer");
        b.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.OrganizationId, x.StartsAt, x.EndsAt });
    }

    public void Configure(EntityTypeBuilder<CrmDiscountCampaignItemRule> b)
    {
        b.ToTable("crm_discount_campaign_item_rule");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.CampaignId).HasColumnName("campaign_id");
        b.Property(x => x.RuleRole).HasColumnName("rule_role").HasMaxLength(12);
        b.Property(x => x.ItemId).HasColumnName("item_id");
        b.Property(x => x.MinQty).HasColumnName("min_qty").HasPrecision(14, 3).HasDefaultValue(1m);
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => x.CampaignId);
        b.HasIndex(x => new { x.CampaignId, x.RuleRole, x.ItemId }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<CrmDiscountCampaignTargetCustomer> b)
    {
        b.ToTable("crm_discount_campaign_target_customer");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.CampaignId).HasColumnName("campaign_id");
        b.Property(x => x.CustomerId).HasColumnName("customer_id");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.CampaignId, x.CustomerId }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<CrmDiscountCampaignUsage> b)
    {
        b.ToTable("crm_discount_campaign_usage");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.CampaignId).HasColumnName("campaign_id");
        b.Property(x => x.CustomerId).HasColumnName("customer_id");
        b.Property(x => x.OrderRef).HasColumnName("order_ref").HasMaxLength(60);
        b.Property(x => x.DiscountAmount).HasColumnName("discount_amount").HasPrecision(14, 2);
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.CampaignId, x.CreatedAt });
    }

    public void Configure(EntityTypeBuilder<CrmDiscountCoupon> b)
    {
        b.ToTable("crm_discount_coupon");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.CampaignId).HasColumnName("campaign_id");
        b.Property(x => x.Code).HasColumnName("code").HasMaxLength(30);
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => x.CampaignId).IsUnique();
        b.HasIndex(x => x.Code).IsUnique();
    }

    public void Configure(EntityTypeBuilder<CrmCustomerFinancialSummary> b)
    {
        b.ToView("crm_customer_financial_summary");
        b.HasNoKey();
        b.Property(x => x.CustomerId).HasColumnName("customer_id");
        b.Property(x => x.OrganizationId).HasColumnName("organization_id");
        b.Property(x => x.CustomerNo).HasColumnName("customer_no");
        b.Property(x => x.FullName).HasColumnName("full_name");
        b.Property(x => x.Phone).HasColumnName("phone");
        b.Property(x => x.BalanceStatus).HasColumnName("balance_status");
        b.Property(x => x.TotalDebitAmount).HasColumnName("total_debit_amount");
        b.Property(x => x.TotalCreditAmount).HasColumnName("total_credit_amount");
        b.Property(x => x.NetBalanceAmount).HasColumnName("net_balance_amount");
        b.Property(x => x.AllowCredit).HasColumnName("allow_credit");
        b.Property(x => x.CreditLimitAmount).HasColumnName("credit_limit_amount");
    }
}
