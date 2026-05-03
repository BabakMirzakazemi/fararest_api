using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase6SchemaAlignmentCoreCrmMenuSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP VIEW IF EXISTS public.support_ticket_metrics;
                DROP VIEW IF EXISTS public.menu_digital_menu_metrics;
                DROP VIEW IF EXISTS public.crm_customer_financial_summary;
                """);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "support_ticket_message",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "author_type",
                table: "support_ticket_message",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                defaultValueSql: "'requester'::character varying",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "event_type",
                table: "support_ticket_event",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "support_ticket_event",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "support_ticket_attachment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "support_ticket",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "ticket_no",
                table: "support_ticket",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "subject_code",
                table: "support_ticket",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "support_ticket",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValueSql: "'open'::character varying",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "priority",
                table: "support_ticket",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValueSql: "'normal'::character varying",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "last_activity_at",
                table: "support_ticket",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "support_ticket",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<decimal>(
                name: "quantity",
                table: "menu_item_ingredient",
                type: "numeric(14,3)",
                precision: 14,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_item_ingredient",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<decimal>(
                name: "quantity",
                table: "menu_ingredient_component",
                type: "numeric(14,3)",
                precision: 14,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_ingredient_component",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "menu_ingredient",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<decimal>(
                name: "price_amount",
                table: "menu_ingredient",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "menu_ingredient",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "is_sellable",
                table: "menu_ingredient",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "ingredient_type",
                table: "menu_ingredient",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_ingredient",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "menu_ingredient",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "brand_name",
                table: "menu_ingredient",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "visited_at",
                table: "menu_digital_menu_visit",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_digital_menu_visit",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "menu_digital_menu_schedule_weekly",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "menu_digital_menu_schedule_weekly",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_digital_menu_schedule_weekly",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "menu_digital_menu_schedule_exception",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "menu_digital_menu_schedule_exception",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_digital_menu_schedule_exception",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "menu_digital_menu_profile",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "theme_name",
                table: "menu_digital_menu_profile",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValueSql: "'default'::character varying",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "menu_digital_menu_profile",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "menu_title",
                table: "menu_digital_menu_profile",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValueSql: "'???? ???????'::character varying",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_digital_menu_profile",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "menu_digital_menu",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "menu_digital_menu",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "show_prices",
                table: "menu_digital_menu",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "show_online_table_reservation",
                table: "menu_digital_menu",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_enabled",
                table: "menu_digital_menu",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "hide_unavailable_items",
                table: "menu_digital_menu",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "enable_dine_in_order",
                table: "menu_digital_menu",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_digital_menu",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "crm_organization_phone",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "crm_organization_phone",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "crm_organization_phone",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_organization_phone",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "crm_loyalty_tier",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "crm_loyalty_tier",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "min_total_spent",
                table: "crm_loyalty_tier",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "crm_loyalty_tier",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_loyalty_tier",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "crm_interest_tag",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "crm_interest_tag",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "crm_interest_tag",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_interest_tag",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_discount_coupon",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "crm_discount_coupon",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "order_ref",
                table: "crm_discount_campaign_usage",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "discount_amount",
                table: "crm_discount_campaign_usage",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_discount_campaign_usage",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_discount_campaign_target_customer",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "rule_role",
                table: "crm_discount_campaign_item_rule",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "min_qty",
                table: "crm_discount_campaign_item_rule",
                type: "numeric(14,3)",
                precision: 14,
                scale: 3,
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_discount_campaign_item_rule",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "crm_discount_campaign",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "crm_discount_campaign",
                type: "character varying(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "min_order_amount",
                table: "crm_discount_campaign",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "max_discount_amount",
                table: "crm_discount_campaign",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "crm_discount_campaign",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<decimal>(
                name: "discount_value",
                table: "crm_discount_campaign",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "discount_type",
                table: "crm_discount_campaign",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValueSql: "'percent'::character varying",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_discount_campaign",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "campaign_type",
                table: "crm_discount_campaign",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "note_type",
                table: "crm_customer_note",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValueSql: "'general'::character varying",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_customer_note",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "crm_customer_loyalty",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_spent_amount",
                table: "crm_customer_loyalty",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_customer_loyalty",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "reference_type",
                table: "crm_customer_ledger",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reference_id",
                table: "crm_customer_ledger",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "entry_type",
                table: "crm_customer_ledger",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_customer_ledger",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "crm_customer_ledger",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_customer_interest",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "crm_customer",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_debit_amount",
                table: "crm_customer",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_credit_amount",
                table: "crm_customer",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "crm_customer",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "payment_behavior",
                table: "crm_customer",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValueSql: "'neutral'::character varying",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "net_balance_amount",
                table: "crm_customer",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "crm_customer",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                table: "crm_customer",
                type: "character varying(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "crm_customer",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "customer_no",
                table: "crm_customer",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "credit_limit_amount",
                table: "crm_customer",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_customer",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "balance_status",
                table: "crm_customer",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValueSql: "'settled'::character varying",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "core_province",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<string>(
                name: "tel_prefix",
                table: "core_province",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "core_province",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "core_province",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "core_province",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "core_province",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "core_county",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "core_county",
                type: "character varying(140)",
                maxLength: 140,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "core_county",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "core_county",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "core_county",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_message_ticket_id_created_at",
                table: "support_ticket_message",
                columns: new[] { "ticket_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_event_ticket_id_created_at",
                table: "support_ticket_event",
                columns: new[] { "ticket_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_attachment_message_id",
                table: "support_ticket_attachment",
                column: "message_id",
                filter: "(message_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_attachment_ticket_id",
                table: "support_ticket_attachment",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_assigned_to_user_id",
                table: "support_ticket",
                column: "assigned_to_user_id",
                filter: "(assigned_to_user_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_organization_id_status_last_activity_at",
                table: "support_ticket",
                columns: new[] { "organization_id", "status", "last_activity_at" });

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_requester_user_id_created_at",
                table: "support_ticket",
                columns: new[] { "requester_user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_support_ticket_ticket_no",
                table: "support_ticket",
                column: "ticket_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_item_ingredient_ingredient_id",
                table: "menu_item_ingredient",
                column: "ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_item_ingredient_item_id",
                table: "menu_item_ingredient",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_item_ingredient_item_id_ingredient_id",
                table: "menu_item_ingredient",
                columns: new[] { "item_id", "ingredient_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_ingredient_component_component_ingredient_id",
                table: "menu_ingredient_component",
                column: "component_ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_ingredient_component_prepared_ingredient_id",
                table: "menu_ingredient_component",
                column: "prepared_ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_ingredient_component_prepared_ingredient_id_component_~",
                table: "menu_ingredient_component",
                columns: new[] { "prepared_ingredient_id", "component_ingredient_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_ingredient_ingredient_type",
                table: "menu_ingredient",
                column: "ingredient_type");

            migrationBuilder.CreateIndex(
                name: "IX_menu_ingredient_organization_id",
                table: "menu_ingredient",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_ingredient_organization_id_code",
                table: "menu_ingredient",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_ingredient_unit_id",
                table: "menu_ingredient",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_digital_menu_visit_digital_menu_id_visited_at",
                table: "menu_digital_menu_visit",
                columns: new[] { "digital_menu_id", "visited_at" });

            migrationBuilder.CreateIndex(
                name: "IX_menu_digital_menu_schedule_weekly_digital_menu_id",
                table: "menu_digital_menu_schedule_weekly",
                column: "digital_menu_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_digital_menu_schedule_weekly_digital_menu_id_weekday",
                table: "menu_digital_menu_schedule_weekly",
                columns: new[] { "digital_menu_id", "weekday" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_digital_menu_schedule_exception_digital_menu_id_date_v~",
                table: "menu_digital_menu_schedule_exception",
                columns: new[] { "digital_menu_id", "date_value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_digital_menu_profile_digital_menu_id",
                table: "menu_digital_menu_profile",
                column: "digital_menu_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_digital_menu_organization_id",
                table: "menu_digital_menu",
                column: "organization_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_digital_menu_token",
                table: "menu_digital_menu",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_organization_phone_organization_id_phone",
                table: "crm_organization_phone",
                columns: new[] { "organization_id", "phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_loyalty_tier_organization_id_name",
                table: "crm_loyalty_tier",
                columns: new[] { "organization_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_loyalty_tier_organization_id_rank_no",
                table: "crm_loyalty_tier",
                columns: new[] { "organization_id", "rank_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_interest_tag_organization_id_name",
                table: "crm_interest_tag",
                columns: new[] { "organization_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_discount_coupon_campaign_id",
                table: "crm_discount_coupon",
                column: "campaign_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_discount_coupon_code",
                table: "crm_discount_coupon",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_discount_campaign_usage_campaign_id_created_at",
                table: "crm_discount_campaign_usage",
                columns: new[] { "campaign_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_crm_discount_campaign_target_customer_campaign_id_customer_~",
                table: "crm_discount_campaign_target_customer",
                columns: new[] { "campaign_id", "customer_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_discount_campaign_item_rule_campaign_id",
                table: "crm_discount_campaign_item_rule",
                column: "campaign_id");

            migrationBuilder.CreateIndex(
                name: "IX_crm_discount_campaign_item_rule_campaign_id_rule_role_item_~",
                table: "crm_discount_campaign_item_rule",
                columns: new[] { "campaign_id", "rule_role", "item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_discount_campaign_organization_id_starts_at_ends_at",
                table: "crm_discount_campaign",
                columns: new[] { "organization_id", "starts_at", "ends_at" });

            migrationBuilder.CreateIndex(
                name: "IX_crm_customer_note_customer_id_created_at",
                table: "crm_customer_note",
                columns: new[] { "customer_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_crm_customer_loyalty_customer_id",
                table: "crm_customer_loyalty",
                column: "customer_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_customer_ledger_customer_id_created_at",
                table: "crm_customer_ledger",
                columns: new[] { "customer_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_crm_customer_interest_customer_id",
                table: "crm_customer_interest",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_crm_customer_interest_customer_id_interest_tag_id",
                table: "crm_customer_interest",
                columns: new[] { "customer_id", "interest_tag_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_customer_interest_interest_tag_id",
                table: "crm_customer_interest",
                column: "interest_tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_crm_customer_organization_id_customer_no",
                table: "crm_customer",
                columns: new[] { "organization_id", "customer_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_crm_customer_organization_id_phone",
                table: "crm_customer",
                columns: new[] { "organization_id", "phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_core_province_name",
                table: "core_province",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_core_province_slug",
                table: "core_province",
                column: "slug",
                unique: true,
                filter: "(slug IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_core_county_province_id",
                table: "core_county",
                column: "province_id");

            migrationBuilder.CreateIndex(
                name: "IX_core_county_province_id_slug",
                table: "core_county",
                columns: new[] { "province_id", "slug" },
                unique: true,
                filter: "(slug IS NOT NULL)");

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
                       count(*) FILTER (WHERE status::text = 'open'::text) AS open_count,
                       count(*) FILTER (WHERE status::text = 'in_review'::text) AS in_review_count,
                       count(*) FILTER (WHERE status::text = 'answered'::text) AS answered_count,
                       count(*) FILTER (WHERE status::text = 'closed'::text) AS closed_count,
                       count(*) AS total_count
                FROM support_ticket
                GROUP BY organization_id;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP VIEW IF EXISTS public.support_ticket_metrics;
                DROP VIEW IF EXISTS public.menu_digital_menu_metrics;
                DROP VIEW IF EXISTS public.crm_customer_financial_summary;
                """);

            migrationBuilder.DropIndex(
                name: "IX_support_ticket_message_ticket_id_created_at",
                table: "support_ticket_message");

            migrationBuilder.DropIndex(
                name: "IX_support_ticket_event_ticket_id_created_at",
                table: "support_ticket_event");

            migrationBuilder.DropIndex(
                name: "IX_support_ticket_attachment_message_id",
                table: "support_ticket_attachment");

            migrationBuilder.DropIndex(
                name: "IX_support_ticket_attachment_ticket_id",
                table: "support_ticket_attachment");

            migrationBuilder.DropIndex(
                name: "IX_support_ticket_assigned_to_user_id",
                table: "support_ticket");

            migrationBuilder.DropIndex(
                name: "IX_support_ticket_organization_id_status_last_activity_at",
                table: "support_ticket");

            migrationBuilder.DropIndex(
                name: "IX_support_ticket_requester_user_id_created_at",
                table: "support_ticket");

            migrationBuilder.DropIndex(
                name: "IX_support_ticket_ticket_no",
                table: "support_ticket");

            migrationBuilder.DropIndex(
                name: "IX_menu_item_ingredient_ingredient_id",
                table: "menu_item_ingredient");

            migrationBuilder.DropIndex(
                name: "IX_menu_item_ingredient_item_id",
                table: "menu_item_ingredient");

            migrationBuilder.DropIndex(
                name: "IX_menu_item_ingredient_item_id_ingredient_id",
                table: "menu_item_ingredient");

            migrationBuilder.DropIndex(
                name: "IX_menu_ingredient_component_component_ingredient_id",
                table: "menu_ingredient_component");

            migrationBuilder.DropIndex(
                name: "IX_menu_ingredient_component_prepared_ingredient_id",
                table: "menu_ingredient_component");

            migrationBuilder.DropIndex(
                name: "IX_menu_ingredient_component_prepared_ingredient_id_component_~",
                table: "menu_ingredient_component");

            migrationBuilder.DropIndex(
                name: "IX_menu_ingredient_ingredient_type",
                table: "menu_ingredient");

            migrationBuilder.DropIndex(
                name: "IX_menu_ingredient_organization_id",
                table: "menu_ingredient");

            migrationBuilder.DropIndex(
                name: "IX_menu_ingredient_organization_id_code",
                table: "menu_ingredient");

            migrationBuilder.DropIndex(
                name: "IX_menu_ingredient_unit_id",
                table: "menu_ingredient");

            migrationBuilder.DropIndex(
                name: "IX_menu_digital_menu_visit_digital_menu_id_visited_at",
                table: "menu_digital_menu_visit");

            migrationBuilder.DropIndex(
                name: "IX_menu_digital_menu_schedule_weekly_digital_menu_id",
                table: "menu_digital_menu_schedule_weekly");

            migrationBuilder.DropIndex(
                name: "IX_menu_digital_menu_schedule_weekly_digital_menu_id_weekday",
                table: "menu_digital_menu_schedule_weekly");

            migrationBuilder.DropIndex(
                name: "IX_menu_digital_menu_schedule_exception_digital_menu_id_date_v~",
                table: "menu_digital_menu_schedule_exception");

            migrationBuilder.DropIndex(
                name: "IX_menu_digital_menu_profile_digital_menu_id",
                table: "menu_digital_menu_profile");

            migrationBuilder.DropIndex(
                name: "IX_menu_digital_menu_organization_id",
                table: "menu_digital_menu");

            migrationBuilder.DropIndex(
                name: "IX_menu_digital_menu_token",
                table: "menu_digital_menu");

            migrationBuilder.DropIndex(
                name: "IX_crm_organization_phone_organization_id_phone",
                table: "crm_organization_phone");

            migrationBuilder.DropIndex(
                name: "IX_crm_loyalty_tier_organization_id_name",
                table: "crm_loyalty_tier");

            migrationBuilder.DropIndex(
                name: "IX_crm_loyalty_tier_organization_id_rank_no",
                table: "crm_loyalty_tier");

            migrationBuilder.DropIndex(
                name: "IX_crm_interest_tag_organization_id_name",
                table: "crm_interest_tag");

            migrationBuilder.DropIndex(
                name: "IX_crm_discount_coupon_campaign_id",
                table: "crm_discount_coupon");

            migrationBuilder.DropIndex(
                name: "IX_crm_discount_coupon_code",
                table: "crm_discount_coupon");

            migrationBuilder.DropIndex(
                name: "IX_crm_discount_campaign_usage_campaign_id_created_at",
                table: "crm_discount_campaign_usage");

            migrationBuilder.DropIndex(
                name: "IX_crm_discount_campaign_target_customer_campaign_id_customer_~",
                table: "crm_discount_campaign_target_customer");

            migrationBuilder.DropIndex(
                name: "IX_crm_discount_campaign_item_rule_campaign_id",
                table: "crm_discount_campaign_item_rule");

            migrationBuilder.DropIndex(
                name: "IX_crm_discount_campaign_item_rule_campaign_id_rule_role_item_~",
                table: "crm_discount_campaign_item_rule");

            migrationBuilder.DropIndex(
                name: "IX_crm_discount_campaign_organization_id_starts_at_ends_at",
                table: "crm_discount_campaign");

            migrationBuilder.DropIndex(
                name: "IX_crm_customer_note_customer_id_created_at",
                table: "crm_customer_note");

            migrationBuilder.DropIndex(
                name: "IX_crm_customer_loyalty_customer_id",
                table: "crm_customer_loyalty");

            migrationBuilder.DropIndex(
                name: "IX_crm_customer_ledger_customer_id_created_at",
                table: "crm_customer_ledger");

            migrationBuilder.DropIndex(
                name: "IX_crm_customer_interest_customer_id",
                table: "crm_customer_interest");

            migrationBuilder.DropIndex(
                name: "IX_crm_customer_interest_customer_id_interest_tag_id",
                table: "crm_customer_interest");

            migrationBuilder.DropIndex(
                name: "IX_crm_customer_interest_interest_tag_id",
                table: "crm_customer_interest");

            migrationBuilder.DropIndex(
                name: "IX_crm_customer_organization_id_customer_no",
                table: "crm_customer");

            migrationBuilder.DropIndex(
                name: "IX_crm_customer_organization_id_phone",
                table: "crm_customer");

            migrationBuilder.DropIndex(
                name: "IX_core_province_name",
                table: "core_province");

            migrationBuilder.DropIndex(
                name: "IX_core_province_slug",
                table: "core_province");

            migrationBuilder.DropIndex(
                name: "IX_core_county_province_id",
                table: "core_county");

            migrationBuilder.DropIndex(
                name: "IX_core_county_province_id_slug",
                table: "core_county");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "support_ticket_message",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "author_type",
                table: "support_ticket_message",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldMaxLength: 12,
                oldDefaultValueSql: "'requester'::character varying");

            migrationBuilder.AlterColumn<string>(
                name: "event_type",
                table: "support_ticket_event",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "support_ticket_event",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "support_ticket_attachment",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "support_ticket",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "ticket_no",
                table: "support_ticket",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(24)",
                oldMaxLength: 24);

            migrationBuilder.AlterColumn<string>(
                name: "subject_code",
                table: "support_ticket",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "support_ticket",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValueSql: "'open'::character varying");

            migrationBuilder.AlterColumn<string>(
                name: "priority",
                table: "support_ticket",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldDefaultValueSql: "'normal'::character varying");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "last_activity_at",
                table: "support_ticket",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "support_ticket",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<decimal>(
                name: "quantity",
                table: "menu_item_ingredient",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,3)",
                oldPrecision: 14,
                oldScale: 3);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_item_ingredient",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<decimal>(
                name: "quantity",
                table: "menu_ingredient_component",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,3)",
                oldPrecision: 14,
                oldScale: 3);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_ingredient_component",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "menu_ingredient",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<decimal>(
                name: "price_amount",
                table: "menu_ingredient",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)",
                oldPrecision: 14,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "menu_ingredient",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<bool>(
                name: "is_sellable",
                table: "menu_ingredient",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "ingredient_type",
                table: "menu_ingredient",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldMaxLength: 12);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_ingredient",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "menu_ingredient",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "brand_name",
                table: "menu_ingredient",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "visited_at",
                table: "menu_digital_menu_visit",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_digital_menu_visit",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "menu_digital_menu_schedule_weekly",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "menu_digital_menu_schedule_weekly",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_digital_menu_schedule_weekly",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "menu_digital_menu_schedule_exception",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "menu_digital_menu_schedule_exception",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_digital_menu_schedule_exception",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "menu_digital_menu_profile",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "theme_name",
                table: "menu_digital_menu_profile",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldDefaultValueSql: "'default'::character varying");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "menu_digital_menu_profile",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "menu_title",
                table: "menu_digital_menu_profile",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120,
                oldDefaultValueSql: "'???? ???????'::character varying");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_digital_menu_profile",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "menu_digital_menu",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "menu_digital_menu",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(24)",
                oldMaxLength: 24);

            migrationBuilder.AlterColumn<bool>(
                name: "show_prices",
                table: "menu_digital_menu",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "show_online_table_reservation",
                table: "menu_digital_menu",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_enabled",
                table: "menu_digital_menu",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "hide_unavailable_items",
                table: "menu_digital_menu",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "enable_dine_in_order",
                table: "menu_digital_menu",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "menu_digital_menu",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "crm_organization_phone",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "crm_organization_phone",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "crm_organization_phone",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_organization_phone",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "crm_loyalty_tier",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "crm_loyalty_tier",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<decimal>(
                name: "min_total_spent",
                table: "crm_loyalty_tier",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)",
                oldPrecision: 14,
                oldScale: 2);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "crm_loyalty_tier",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_loyalty_tier",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "crm_interest_tag",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "crm_interest_tag",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "crm_interest_tag",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_interest_tag",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_discount_coupon",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "crm_discount_coupon",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "order_ref",
                table: "crm_discount_campaign_usage",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "discount_amount",
                table: "crm_discount_campaign_usage",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)",
                oldPrecision: 14,
                oldScale: 2);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_discount_campaign_usage",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_discount_campaign_target_customer",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "rule_role",
                table: "crm_discount_campaign_item_rule",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldMaxLength: 12);

            migrationBuilder.AlterColumn<decimal>(
                name: "min_qty",
                table: "crm_discount_campaign_item_rule",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,3)",
                oldPrecision: 14,
                oldScale: 3,
                oldDefaultValue: 1m);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_discount_campaign_item_rule",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "crm_discount_campaign",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "crm_discount_campaign",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<decimal>(
                name: "min_order_amount",
                table: "crm_discount_campaign",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)",
                oldPrecision: 14,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "max_discount_amount",
                table: "crm_discount_campaign",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)",
                oldPrecision: 14,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "crm_discount_campaign",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "discount_value",
                table: "crm_discount_campaign",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "discount_type",
                table: "crm_discount_campaign",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldDefaultValueSql: "'percent'::character varying");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_discount_campaign",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "campaign_type",
                table: "crm_discount_campaign",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "note_type",
                table: "crm_customer_note",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValueSql: "'general'::character varying");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_customer_note",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "crm_customer_loyalty",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_spent_amount",
                table: "crm_customer_loyalty",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)",
                oldPrecision: 14,
                oldScale: 2);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_customer_loyalty",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "reference_type",
                table: "crm_customer_ledger",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reference_id",
                table: "crm_customer_ledger",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "entry_type",
                table: "crm_customer_ledger",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_customer_ledger",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "crm_customer_ledger",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)",
                oldPrecision: 14,
                oldScale: 2);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_customer_interest",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "crm_customer",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_debit_amount",
                table: "crm_customer",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)",
                oldPrecision: 14,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "total_credit_amount",
                table: "crm_customer",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)",
                oldPrecision: 14,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "crm_customer",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "payment_behavior",
                table: "crm_customer",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldDefaultValueSql: "'neutral'::character varying");

            migrationBuilder.AlterColumn<decimal>(
                name: "net_balance_amount",
                table: "crm_customer",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)",
                oldPrecision: 14,
                oldScale: 2);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "crm_customer",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                table: "crm_customer",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "crm_customer",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(254)",
                oldMaxLength: 254);

            migrationBuilder.AlterColumn<string>(
                name: "customer_no",
                table: "crm_customer",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(24)",
                oldMaxLength: 24);

            migrationBuilder.AlterColumn<decimal>(
                name: "credit_limit_amount",
                table: "crm_customer",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,2)",
                oldPrecision: 14,
                oldScale: 2);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "crm_customer",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "balance_status",
                table: "crm_customer",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldDefaultValueSql: "'settled'::character varying");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "core_province",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "tel_prefix",
                table: "core_province",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "core_province",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "core_province",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "core_province",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "core_province",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "core_county",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "core_county",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(140)",
                oldMaxLength: 140,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "core_county",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "core_county",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "core_county",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
