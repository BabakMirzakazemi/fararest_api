using System;
using System.Net;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase5CoreCrmMenuSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "core_county",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    province_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    slug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_county", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "core_province",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    slug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    tel_prefix = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_province", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_customer",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    customer_no = table.Column<string>(type: "text", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: true),
                    allow_credit = table.Column<bool>(type: "boolean", nullable: false),
                    credit_limit_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_debit_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_credit_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    net_balance_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    balance_status = table.Column<string>(type: "text", nullable: false),
                    payment_behavior = table.Column<string>(type: "text", nullable: false),
                    is_loyal_customer = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_activity_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    allow_installment = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_customer", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_customer_interest",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    interest_tag_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_customer_interest", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_customer_ledger",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    entry_type = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    reference_type = table.Column<string>(type: "text", nullable: true),
                    reference_id = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_customer_ledger", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_customer_loyalty",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    tier_id = table.Column<long>(type: "bigint", nullable: true),
                    points_balance = table.Column<int>(type: "integer", nullable: false),
                    total_spent_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_visit_count = table.Column<int>(type: "integer", nullable: false),
                    last_visit_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_customer_loyalty", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_customer_note",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    note_type = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    is_important = table.Column<bool>(type: "boolean", nullable: false),
                    created_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_customer_note", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_discount_campaign",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    campaign_type = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    starts_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ends_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    discount_type = table.Column<string>(type: "text", nullable: false),
                    discount_value = table.Column<decimal>(type: "numeric", nullable: false),
                    min_order_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    max_discount_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    usage_limit_total = table.Column<int>(type: "integer", nullable: true),
                    usage_limit_per_customer = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_discount_campaign", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_discount_campaign_item_rule",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    campaign_id = table.Column<long>(type: "bigint", nullable: false),
                    rule_role = table.Column<string>(type: "text", nullable: false),
                    item_id = table.Column<long>(type: "bigint", nullable: false),
                    min_qty = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_discount_campaign_item_rule", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_discount_campaign_target_customer",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    campaign_id = table.Column<long>(type: "bigint", nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_discount_campaign_target_customer", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_discount_campaign_usage",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    campaign_id = table.Column<long>(type: "bigint", nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: true),
                    order_ref = table.Column<string>(type: "text", nullable: true),
                    discount_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_discount_campaign_usage", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_discount_coupon",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    campaign_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_discount_coupon", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_interest_tag",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_interest_tag", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_loyalty_tier",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    rank_no = table.Column<short>(type: "smallint", nullable: false),
                    min_points = table.Column<int>(type: "integer", nullable: false),
                    min_total_spent = table.Column<decimal>(type: "numeric", nullable: false),
                    benefits_description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_loyalty_tier", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crm_organization_phone",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crm_organization_phone", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "menu_digital_menu",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    public_url = table.Column<string>(type: "text", nullable: false),
                    qr_code_url = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    show_prices = table.Column<bool>(type: "boolean", nullable: false),
                    show_online_table_reservation = table.Column<bool>(type: "boolean", nullable: false),
                    enable_dine_in_order = table.Column<bool>(type: "boolean", nullable: false),
                    enable_delivery_order = table.Column<bool>(type: "boolean", nullable: false),
                    hide_unavailable_items = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_digital_menu", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "menu_digital_menu_profile",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    digital_menu_id = table.Column<long>(type: "bigint", nullable: false),
                    theme_name = table.Column<string>(type: "text", nullable: false),
                    menu_title = table.Column<string>(type: "text", nullable: false),
                    menu_description = table.Column<string>(type: "text", nullable: true),
                    logo_url = table.Column<string>(type: "text", nullable: true),
                    header_image_url = table.Column<string>(type: "text", nullable: true),
                    header_video_url = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    social_telegram_url = table.Column<string>(type: "text", nullable: true),
                    social_instagram_url = table.Column<string>(type: "text", nullable: true),
                    social_website_url = table.Column<string>(type: "text", nullable: true),
                    social_rubika_url = table.Column<string>(type: "text", nullable: true),
                    social_eitaa_url = table.Column<string>(type: "text", nullable: true),
                    social_bale_url = table.Column<string>(type: "text", nullable: true),
                    social_whatsapp_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_digital_menu_profile", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "menu_digital_menu_schedule_exception",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    digital_menu_id = table.Column<long>(type: "bigint", nullable: false),
                    date_value = table.Column<DateOnly>(type: "date", nullable: false),
                    is_closed_all_day = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    open_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    close_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_digital_menu_schedule_exception", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "menu_digital_menu_schedule_weekly",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    digital_menu_id = table.Column<long>(type: "bigint", nullable: false),
                    weekday = table.Column<short>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    open_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    close_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_digital_menu_schedule_weekly", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "menu_digital_menu_visit",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    digital_menu_id = table.Column<long>(type: "bigint", nullable: false),
                    visited_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    visitor_key_hash = table.Column<string>(type: "char(64)", nullable: true),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    referrer_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_digital_menu_visit", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "menu_ingredient",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    ingredient_type = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    unit_id = table.Column<long>(type: "bigint", nullable: false),
                    price_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    expiration_date = table.Column<DateOnly>(type: "date", nullable: true),
                    brand_name = table.Column<string>(type: "text", nullable: true),
                    is_sellable = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_urls = table.Column<string[]>(type: "text[]", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_ingredient", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "menu_ingredient_component",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    prepared_ingredient_id = table.Column<long>(type: "bigint", nullable: false),
                    component_ingredient_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    unit_id = table.Column<long>(type: "bigint", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_ingredient_component", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "menu_item_ingredient",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<long>(type: "bigint", nullable: false),
                    ingredient_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    unit_id = table.Column<long>(type: "bigint", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_item_ingredient", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "support_ticket",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ticket_no = table.Column<string>(type: "text", nullable: false),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    requester_user_id = table.Column<int>(type: "integer", nullable: false),
                    subject_code = table.Column<string>(type: "text", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    priority = table.Column<string>(type: "text", nullable: false),
                    assigned_to_user_id = table.Column<int>(type: "integer", nullable: true),
                    first_response_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    resolved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_activity_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    reopen_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_ticket", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "support_ticket_attachment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ticket_id = table.Column<long>(type: "bigint", nullable: false),
                    message_id = table.Column<long>(type: "bigint", nullable: true),
                    file_url = table.Column<string>(type: "text", nullable: false),
                    uploaded_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_ticket_attachment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "support_ticket_event",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ticket_id = table.Column<long>(type: "bigint", nullable: false),
                    event_type = table.Column<string>(type: "text", nullable: false),
                    old_value = table.Column<string>(type: "text", nullable: true),
                    new_value = table.Column<string>(type: "text", nullable: true),
                    actor_user_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_ticket_event", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "support_ticket_message",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ticket_id = table.Column<long>(type: "bigint", nullable: false),
                    author_user_id = table.Column<int>(type: "integer", nullable: true),
                    author_type = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    is_internal = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_ticket_message", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_core_county_province_id_name",
                table: "core_county",
                columns: new[] { "province_id", "name" },
                unique: true);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE VIEW public.crm_customer_financial_summary AS
                SELECT id AS customer_id, organization_id, customer_no, full_name, phone, balance_status,
                       total_debit_amount, total_credit_amount, net_balance_amount, allow_credit, credit_limit_amount
                FROM crm_customer;
                """);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE VIEW public.menu_digital_menu_metrics AS
                SELECT dm.id AS digital_menu_id, dm.organization_id,
                       0::bigint AS visits_today, 0::bigint AS visits_week,
                       0::bigint AS category_count, 0::bigint AS item_count
                FROM menu_digital_menu dm;
                """);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE VIEW public.support_ticket_metrics AS
                SELECT organization_id,
                       count(*) FILTER (WHERE status = 'open') AS open_count,
                       count(*) FILTER (WHERE status = 'in_review') AS in_review_count,
                       count(*) FILTER (WHERE status = 'answered') AS answered_count,
                       count(*) FILTER (WHERE status = 'closed') AS closed_count,
                       count(*) AS total_count
                FROM support_ticket
                GROUP BY organization_id;
                """);

            migrationBuilder.Sql(ReadEmbeddedSql("phase5_db_objects_up.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ReadEmbeddedSql("phase5_db_objects_down.sql"));

            migrationBuilder.DropTable(
                name: "core_county");

            migrationBuilder.DropTable(
                name: "core_province");

            migrationBuilder.DropTable(
                name: "crm_customer");

            migrationBuilder.DropTable(
                name: "crm_customer_interest");

            migrationBuilder.DropTable(
                name: "crm_customer_ledger");

            migrationBuilder.DropTable(
                name: "crm_customer_loyalty");

            migrationBuilder.DropTable(
                name: "crm_customer_note");

            migrationBuilder.DropTable(
                name: "crm_discount_campaign");

            migrationBuilder.DropTable(
                name: "crm_discount_campaign_item_rule");

            migrationBuilder.DropTable(
                name: "crm_discount_campaign_target_customer");

            migrationBuilder.DropTable(
                name: "crm_discount_campaign_usage");

            migrationBuilder.DropTable(
                name: "crm_discount_coupon");

            migrationBuilder.DropTable(
                name: "crm_interest_tag");

            migrationBuilder.DropTable(
                name: "crm_loyalty_tier");

            migrationBuilder.DropTable(
                name: "crm_organization_phone");

            migrationBuilder.DropTable(
                name: "menu_digital_menu");

            migrationBuilder.DropTable(
                name: "menu_digital_menu_profile");

            migrationBuilder.DropTable(
                name: "menu_digital_menu_schedule_exception");

            migrationBuilder.DropTable(
                name: "menu_digital_menu_schedule_weekly");

            migrationBuilder.DropTable(
                name: "menu_digital_menu_visit");

            migrationBuilder.DropTable(
                name: "menu_ingredient");

            migrationBuilder.DropTable(
                name: "menu_ingredient_component");

            migrationBuilder.DropTable(
                name: "menu_item_ingredient");

            migrationBuilder.DropTable(
                name: "support_ticket");

            migrationBuilder.DropTable(
                name: "support_ticket_attachment");

            migrationBuilder.DropTable(
                name: "support_ticket_event");

            migrationBuilder.DropTable(
                name: "support_ticket_message");
        }

        private static string ReadEmbeddedSql(string fileName)
        {
            var assembly = typeof(Phase5CoreCrmMenuSupport).Assembly;
            var resourceName = $"Data.Migrations.Sql.{fileName}";
            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Embedded SQL resource not found: {resourceName}");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
