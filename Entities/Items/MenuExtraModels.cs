using System.Net;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Items;

public class MenuIngredient : BaseEntity<long>, IEntity
{
    public long OrganizationId { get; set; }
    public string IngredientType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public long UnitId { get; set; }
    public decimal PriceAmount { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public string? BrandName { get; set; }
    public bool IsSellable { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public string[]? ImageUrls { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class MenuIngredientComponent : BaseEntity<long>, IEntity
{
    public long PreparedIngredientId { get; set; }
    public long ComponentIngredientId { get; set; }
    public decimal Quantity { get; set; }
    public long UnitId { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class MenuItemIngredient : BaseEntity<long>, IEntity
{
    public long ItemId { get; set; }
    public long IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public long UnitId { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class MenuDigitalMenu : BaseEntity<long>, IEntity
{
    public long OrganizationId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public string? QrCodeUrl { get; set; }
    public bool IsEnabled { get; set; }
    public bool ShowPrices { get; set; }
    public bool ShowOnlineTableReservation { get; set; }
    public bool EnableDineInOrder { get; set; }
    public bool EnableDeliveryOrder { get; set; }
    public bool HideUnavailableItems { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class MenuDigitalMenuProfile : BaseEntity<long>, IEntity
{
    public long DigitalMenuId { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    public string MenuTitle { get; set; } = string.Empty;
    public string? MenuDescription { get; set; }
    public string? LogoUrl { get; set; }
    public string? HeaderImageUrl { get; set; }
    public string? HeaderVideoUrl { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? SocialTelegramUrl { get; set; }
    public string? SocialInstagramUrl { get; set; }
    public string? SocialWebsiteUrl { get; set; }
    public string? SocialRubikaUrl { get; set; }
    public string? SocialEitaaUrl { get; set; }
    public string? SocialBaleUrl { get; set; }
    public string? SocialWhatsappUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class MenuDigitalMenuScheduleWeekly : BaseEntity<long>, IEntity
{
    public long DigitalMenuId { get; set; }
    public short Weekday { get; set; }
    public bool IsActive { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class MenuDigitalMenuScheduleException : BaseEntity<long>, IEntity
{
    public long DigitalMenuId { get; set; }
    public DateOnly DateValue { get; set; }
    public bool IsClosedAllDay { get; set; }
    public bool IsActive { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class MenuDigitalMenuVisit : BaseEntity<long>, IEntity
{
    public long DigitalMenuId { get; set; }
    public DateTimeOffset VisitedAt { get; set; }
    public string? VisitorKeyHash { get; set; }
    public IPAddress? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? ReferrerUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class MenuDigitalMenuMetrics : IEntity
{
    public long? DigitalMenuId { get; set; }
    public long? OrganizationId { get; set; }
    public long? VisitsToday { get; set; }
    public long? VisitsWeek { get; set; }
    public long? CategoryCount { get; set; }
    public long? ItemCount { get; set; }
}

public class MenuExtraConfiguration :
    IEntityTypeConfiguration<MenuIngredient>, IEntityTypeConfiguration<MenuIngredientComponent>,
    IEntityTypeConfiguration<MenuItemIngredient>, IEntityTypeConfiguration<MenuDigitalMenu>,
    IEntityTypeConfiguration<MenuDigitalMenuProfile>, IEntityTypeConfiguration<MenuDigitalMenuScheduleWeekly>,
    IEntityTypeConfiguration<MenuDigitalMenuScheduleException>, IEntityTypeConfiguration<MenuDigitalMenuVisit>,
    IEntityTypeConfiguration<MenuDigitalMenuMetrics>
{
    public void Configure(EntityTypeBuilder<MenuIngredient> b)
    {
        b.ToTable("menu_ingredient");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.OrganizationId).HasColumnName("organization_id");
        b.Property(x => x.IngredientType).HasColumnName("ingredient_type").HasMaxLength(12);
        b.Property(x => x.Name).HasColumnName("name").HasMaxLength(120);
        b.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
        b.Property(x => x.UnitId).HasColumnName("unit_id");
        b.Property(x => x.PriceAmount).HasColumnName("price_amount").HasPrecision(14, 2);
        b.Property(x => x.ExpirationDate).HasColumnName("expiration_date");
        b.Property(x => x.BrandName).HasColumnName("brand_name").HasMaxLength(120);
        b.Property(x => x.IsSellable).HasColumnName("is_sellable").HasDefaultValue(true);
        b.Property(x => x.IsActive).HasColumnName("is_active");
        b.Property(x => x.Description).HasColumnName("description");
        b.Property(x => x.ImageUrls).HasColumnName("image_urls");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.OrganizationId, x.Code }).IsUnique();
        b.HasIndex(x => x.OrganizationId);
        b.HasIndex(x => x.IngredientType);
        b.HasIndex(x => x.UnitId);
    }

    public void Configure(EntityTypeBuilder<MenuIngredientComponent> b)
    {
        b.ToTable("menu_ingredient_component");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.PreparedIngredientId).HasColumnName("prepared_ingredient_id");
        b.Property(x => x.ComponentIngredientId).HasColumnName("component_ingredient_id");
        b.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(14, 3);
        b.Property(x => x.UnitId).HasColumnName("unit_id");
        b.Property(x => x.Notes).HasColumnName("notes");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => x.ComponentIngredientId);
        b.HasIndex(x => x.PreparedIngredientId);
        b.HasIndex(x => new { x.PreparedIngredientId, x.ComponentIngredientId }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<MenuItemIngredient> b)
    {
        b.ToTable("menu_item_ingredient");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.ItemId).HasColumnName("item_id");
        b.Property(x => x.IngredientId).HasColumnName("ingredient_id");
        b.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(14, 3);
        b.Property(x => x.UnitId).HasColumnName("unit_id");
        b.Property(x => x.Notes).HasColumnName("notes");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => x.IngredientId);
        b.HasIndex(x => x.ItemId);
        b.HasIndex(x => new { x.ItemId, x.IngredientId }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<MenuDigitalMenu> b)
    {
        b.ToTable("menu_digital_menu");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.OrganizationId).HasColumnName("organization_id");
        b.Property(x => x.Token).HasColumnName("token").HasMaxLength(24);
        b.Property(x => x.PublicUrl).HasColumnName("public_url");
        b.Property(x => x.QrCodeUrl).HasColumnName("qr_code_url");
        b.Property(x => x.IsEnabled).HasColumnName("is_enabled").HasDefaultValue(true);
        b.Property(x => x.ShowPrices).HasColumnName("show_prices").HasDefaultValue(true);
        b.Property(x => x.ShowOnlineTableReservation).HasColumnName("show_online_table_reservation").HasDefaultValue(true);
        b.Property(x => x.EnableDineInOrder).HasColumnName("enable_dine_in_order").HasDefaultValue(true);
        b.Property(x => x.EnableDeliveryOrder).HasColumnName("enable_delivery_order");
        b.Property(x => x.HideUnavailableItems).HasColumnName("hide_unavailable_items").HasDefaultValue(true);
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
        b.HasIndex(x => x.OrganizationId).IsUnique();
        b.HasIndex(x => x.Token).IsUnique();
    }

    public void Configure(EntityTypeBuilder<MenuDigitalMenuProfile> b)
    {
        b.ToTable("menu_digital_menu_profile");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.DigitalMenuId).HasColumnName("digital_menu_id");
        b.Property(x => x.ThemeName).HasColumnName("theme_name").HasMaxLength(80).HasDefaultValueSql("'default'::character varying");
        b.Property(x => x.MenuTitle).HasColumnName("menu_title").HasMaxLength(120).HasDefaultValueSql("'???? ???????'::character varying");
        b.Property(x => x.MenuDescription).HasColumnName("menu_description");
        b.Property(x => x.LogoUrl).HasColumnName("logo_url");
        b.Property(x => x.HeaderImageUrl).HasColumnName("header_image_url");
        b.Property(x => x.HeaderVideoUrl).HasColumnName("header_video_url");
        b.Property(x => x.Address).HasColumnName("address");
        b.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20);
        b.Property(x => x.SocialTelegramUrl).HasColumnName("social_telegram_url");
        b.Property(x => x.SocialInstagramUrl).HasColumnName("social_instagram_url");
        b.Property(x => x.SocialWebsiteUrl).HasColumnName("social_website_url");
        b.Property(x => x.SocialRubikaUrl).HasColumnName("social_rubika_url");
        b.Property(x => x.SocialEitaaUrl).HasColumnName("social_eitaa_url");
        b.Property(x => x.SocialBaleUrl).HasColumnName("social_bale_url");
        b.Property(x => x.SocialWhatsappUrl).HasColumnName("social_whatsapp_url");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
        b.HasIndex(x => x.DigitalMenuId).IsUnique();
    }

    public void Configure(EntityTypeBuilder<MenuDigitalMenuScheduleWeekly> b)
    {
        b.ToTable("menu_digital_menu_schedule_weekly");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.DigitalMenuId).HasColumnName("digital_menu_id");
        b.Property(x => x.Weekday).HasColumnName("weekday");
        b.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        b.Property(x => x.OpenTime).HasColumnName("open_time");
        b.Property(x => x.CloseTime).HasColumnName("close_time");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
        b.HasIndex(x => x.DigitalMenuId);
        b.HasIndex(x => new { x.DigitalMenuId, x.Weekday }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<MenuDigitalMenuScheduleException> b)
    {
        b.ToTable("menu_digital_menu_schedule_exception");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.DigitalMenuId).HasColumnName("digital_menu_id");
        b.Property(x => x.DateValue).HasColumnName("date_value");
        b.Property(x => x.IsClosedAllDay).HasColumnName("is_closed_all_day");
        b.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        b.Property(x => x.OpenTime).HasColumnName("open_time");
        b.Property(x => x.CloseTime).HasColumnName("close_time");
        b.Property(x => x.Note).HasColumnName("note");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.DigitalMenuId, x.DateValue }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<MenuDigitalMenuVisit> b)
    {
        b.ToTable("menu_digital_menu_visit");
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.DigitalMenuId).HasColumnName("digital_menu_id");
        b.Property(x => x.VisitedAt).HasColumnName("visited_at").HasDefaultValueSql("now()");
        b.Property(x => x.VisitorKeyHash).HasColumnName("visitor_key_hash").HasColumnType("char(64)").HasMaxLength(64);
        b.Property(x => x.IpAddress).HasColumnName("ip_address");
        b.Property(x => x.UserAgent).HasColumnName("user_agent");
        b.Property(x => x.ReferrerUrl).HasColumnName("referrer_url");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.HasIndex(x => new { x.DigitalMenuId, x.VisitedAt });
    }

    public void Configure(EntityTypeBuilder<MenuDigitalMenuMetrics> b)
    {
        b.ToView("menu_digital_menu_metrics");
        b.HasNoKey();
        b.Property(x => x.DigitalMenuId).HasColumnName("digital_menu_id");
        b.Property(x => x.OrganizationId).HasColumnName("organization_id");
        b.Property(x => x.VisitsToday).HasColumnName("visits_today");
        b.Property(x => x.VisitsWeek).HasColumnName("visits_week");
        b.Property(x => x.CategoryCount).HasColumnName("category_count");
        b.Property(x => x.ItemCount).HasColumnName("item_count");
    }
}
