using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase3Payments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payments_operation",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    operation_uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    idempotency_key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    operation_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    purpose_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    payment_mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    provider_reference = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    requested_amount = table.Column<decimal>(type: "numeric(16,2)", nullable: false),
                    wallet_used_amount = table.Column<decimal>(type: "numeric(16,2)", nullable: false, defaultValue: 0m),
                    gateway_charged_amount = table.Column<decimal>(type: "numeric(16,2)", nullable: false, defaultValue: 0m),
                    final_debit_amount = table.Column<decimal>(type: "numeric(16,2)", nullable: false, defaultValue: 0m),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    created_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments_operation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payments_wallet",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    currency_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false, defaultValue: "IRR"),
                    balance_amount = table.Column<decimal>(type: "numeric(16,2)", nullable: false, defaultValue: 0m),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments_wallet", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payments_wallet_entry",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wallet_id = table.Column<long>(type: "bigint", nullable: false),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    operation_id = table.Column<long>(type: "bigint", nullable: false),
                    entry_side = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    entry_kind = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    source_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(16,2)", nullable: false),
                    balance_before = table.Column<decimal>(type: "numeric(16,2)", nullable: false),
                    balance_after = table.Column<decimal>(type: "numeric(16,2)", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    created_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments_wallet_entry", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payments_operation_operation_uuid",
                table: "payments_operation",
                column: "operation_uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_wallet_organization_id",
                table: "payments_wallet",
                column: "organization_id",
                unique: true);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION public.payments_wallet_entry_immutable()
                 RETURNS trigger
                 LANGUAGE plpgsql
                AS $function$
                BEGIN
                    RAISE EXCEPTION 'payments_wallet_entry is immutable; UPDATE/DELETE is not allowed';
                END;
                $function$;

                CREATE OR REPLACE FUNCTION public.set_row_updated_at()
                 RETURNS trigger
                 LANGUAGE plpgsql
                AS $function$
                BEGIN
                    NEW.updated_at := NOW();
                    RETURN NEW;
                END;
                $function$;

                CREATE OR REPLACE FUNCTION public.payments_get_wallet_balance(p_organization_id bigint)
                 RETURNS numeric
                 LANGUAGE sql
                 STABLE
                AS $function$
                    SELECT COALESCE((
                        SELECT w.balance_amount
                          FROM public.payments_wallet w
                         WHERE w.organization_id = p_organization_id
                    ), 0::NUMERIC(16,2))
                $function$;

                CREATE OR REPLACE FUNCTION public.payments_get_or_create_wallet(p_organization_id bigint)
                 RETURNS bigint
                 LANGUAGE plpgsql
                AS $function$
                DECLARE
                    v_wallet_id BIGINT;
                BEGIN
                    IF p_organization_id IS NULL THEN
                        RAISE EXCEPTION 'Organization id is required';
                    END IF;

                    SELECT id
                      INTO v_wallet_id
                      FROM public.payments_wallet
                     WHERE organization_id = p_organization_id
                     FOR UPDATE;

                    IF v_wallet_id IS NULL THEN
                        INSERT INTO public.payments_wallet(organization_id)
                        VALUES (p_organization_id)
                        ON CONFLICT (organization_id) DO NOTHING;

                        SELECT id
                          INTO v_wallet_id
                          FROM public.payments_wallet
                         WHERE organization_id = p_organization_id
                         FOR UPDATE;
                    END IF;

                    RETURN v_wallet_id;
                END;
                $function$;

                CREATE OR REPLACE FUNCTION public.payments_apply_wallet_entry(p_organization_id bigint, p_operation_id bigint, p_entry_side character varying, p_entry_kind character varying, p_source_code character varying, p_amount numeric, p_description text DEFAULT NULL::text, p_created_by_user_id integer DEFAULT NULL::integer, p_metadata jsonb DEFAULT NULL::jsonb)
                 RETURNS TABLE(entry_id bigint, balance_after numeric)
                 LANGUAGE plpgsql
                AS $function$
                DECLARE
                    v_wallet_id BIGINT;
                    v_before NUMERIC(16,2);
                    v_after NUMERIC(16,2);
                BEGIN
                    IF p_amount IS NULL OR p_amount <= 0 THEN
                        RAISE EXCEPTION 'Wallet entry amount must be greater than zero';
                    END IF;

                    IF p_entry_side NOT IN ('credit','debit') THEN
                        RAISE EXCEPTION 'Invalid entry side: %', p_entry_side;
                    END IF;

                    v_wallet_id := public.payments_get_or_create_wallet(p_organization_id);

                    SELECT balance_amount
                      INTO v_before
                      FROM public.payments_wallet
                     WHERE id = v_wallet_id
                     FOR UPDATE;

                    IF p_entry_side = 'debit' THEN
                        IF v_before < p_amount THEN
                            RAISE EXCEPTION 'Insufficient wallet balance. current=%, required=%', v_before, p_amount;
                        END IF;
                        v_after := v_before - p_amount;
                    ELSE
                        v_after := v_before + p_amount;
                    END IF;

                    UPDATE public.payments_wallet
                       SET balance_amount = v_after,
                           updated_at = NOW()
                     WHERE id = v_wallet_id;

                    INSERT INTO public.payments_wallet_entry(
                        wallet_id, organization_id, operation_id, entry_side, entry_kind, source_code, amount,
                        balance_before, balance_after, description, metadata, created_by_user_id
                    )
                    VALUES (
                        v_wallet_id, p_organization_id, p_operation_id, p_entry_side, p_entry_kind, p_source_code, p_amount,
                        v_before, v_after, NULLIF(trim(COALESCE(p_description, '')), ''), p_metadata, p_created_by_user_id
                    )
                    RETURNING id, payments_wallet_entry.balance_after
                    INTO entry_id, balance_after;

                    RETURN NEXT;
                END;
                $function$;

                CREATE OR REPLACE FUNCTION public.payments_credit_wallet_gateway(p_organization_id bigint, p_amount numeric, p_provider character varying, p_provider_reference character varying, p_idempotency_key character varying DEFAULT NULL::character varying, p_purpose_code character varying DEFAULT 'wallet_topup_gateway'::character varying, p_description text DEFAULT NULL::text, p_created_by_user_id integer DEFAULT NULL::integer, p_metadata jsonb DEFAULT NULL::jsonb)
                 RETURNS TABLE(operation_id bigint, wallet_balance_after numeric)
                 LANGUAGE plpgsql
                AS $function$
                DECLARE
                    v_operation_id BIGINT;
                    v_existing_status VARCHAR(12);
                BEGIN
                    IF p_amount IS NULL OR p_amount <= 0 THEN
                        RAISE EXCEPTION 'Topup amount must be greater than zero';
                    END IF;

                    IF COALESCE(trim(p_provider), '') = '' THEN
                        RAISE EXCEPTION 'Provider is required for gateway topup';
                    END IF;

                    IF COALESCE(trim(p_provider_reference), '') = '' THEN
                        RAISE EXCEPTION 'Provider reference is required for gateway topup';
                    END IF;

                    IF p_idempotency_key IS NOT NULL THEN
                        SELECT id, status
                          INTO v_operation_id, v_existing_status
                          FROM public.payments_operation
                         WHERE organization_id = p_organization_id
                           AND idempotency_key = p_idempotency_key
                         LIMIT 1;

                        IF v_operation_id IS NOT NULL THEN
                            IF v_existing_status <> 'completed' THEN
                                RAISE EXCEPTION 'Existing operation with same idempotency key is not completed';
                            END IF;
                            operation_id := v_operation_id;
                            wallet_balance_after := public.payments_get_wallet_balance(p_organization_id);
                            RETURN NEXT;
                            RETURN;
                        END IF;
                    END IF;

                    INSERT INTO public.payments_operation(
                        organization_id, idempotency_key, operation_type, purpose_code, payment_mode, provider,
                        provider_reference, requested_amount, gateway_charged_amount, status, description, metadata, created_by_user_id
                    )
                    VALUES (
                        p_organization_id, p_idempotency_key, 'wallet_topup', p_purpose_code, 'gateway', lower(trim(p_provider)),
                        trim(p_provider_reference), p_amount, p_amount, 'pending', p_description, p_metadata, p_created_by_user_id
                    )
                    RETURNING id INTO v_operation_id;

                    SELECT pwe.balance_after
                      INTO wallet_balance_after
                      FROM public.payments_apply_wallet_entry(
                            p_organization_id, v_operation_id, 'credit', 'topup', p_purpose_code, p_amount,
                            p_description, p_created_by_user_id, p_metadata
                      ) AS pwe;

                    UPDATE public.payments_operation
                       SET status = 'completed',
                           wallet_used_amount = 0,
                           final_debit_amount = 0,
                           updated_at = NOW()
                     WHERE id = v_operation_id;

                    operation_id := v_operation_id;
                    RETURN NEXT;
                END;
                $function$;

                CREATE OR REPLACE FUNCTION public.payments_credit_wallet_refund(p_organization_id bigint, p_amount numeric, p_source_code character varying DEFAULT 'plan_downgrade_refund'::character varying, p_idempotency_key character varying DEFAULT NULL::character varying, p_description text DEFAULT NULL::text, p_created_by_user_id integer DEFAULT NULL::integer, p_metadata jsonb DEFAULT NULL::jsonb)
                 RETURNS TABLE(operation_id bigint, wallet_balance_after numeric)
                 LANGUAGE plpgsql
                AS $function$
                DECLARE
                    v_operation_id BIGINT;
                    v_existing_status VARCHAR(12);
                BEGIN
                    IF p_amount IS NULL OR p_amount <= 0 THEN
                        RAISE EXCEPTION 'Refund amount must be greater than zero';
                    END IF;

                    IF p_idempotency_key IS NOT NULL THEN
                        SELECT id, status
                          INTO v_operation_id, v_existing_status
                          FROM public.payments_operation
                         WHERE organization_id = p_organization_id
                           AND idempotency_key = p_idempotency_key
                         LIMIT 1;

                        IF v_operation_id IS NOT NULL THEN
                            IF v_existing_status <> 'completed' THEN
                                RAISE EXCEPTION 'Existing operation with same idempotency key is not completed';
                            END IF;
                            operation_id := v_operation_id;
                            wallet_balance_after := public.payments_get_wallet_balance(p_organization_id);
                            RETURN NEXT;
                            RETURN;
                        END IF;
                    END IF;

                    INSERT INTO public.payments_operation(
                        organization_id, idempotency_key, operation_type, purpose_code, payment_mode, requested_amount,
                        status, description, metadata, created_by_user_id
                    )
                    VALUES (
                        p_organization_id, p_idempotency_key, 'refund', p_source_code, 'system', p_amount,
                        'pending', p_description, p_metadata, p_created_by_user_id
                    )
                    RETURNING id INTO v_operation_id;

                    SELECT pwe.balance_after
                      INTO wallet_balance_after
                      FROM public.payments_apply_wallet_entry(
                            p_organization_id, v_operation_id, 'credit', 'refund', p_source_code, p_amount,
                            p_description, p_created_by_user_id, p_metadata
                      ) AS pwe;

                    UPDATE public.payments_operation
                       SET status = 'completed',
                           gateway_charged_amount = 0,
                           wallet_used_amount = 0,
                           final_debit_amount = 0,
                           updated_at = NOW()
                     WHERE id = v_operation_id;

                    operation_id := v_operation_id;
                    RETURN NEXT;
                END;
                $function$;

                CREATE OR REPLACE FUNCTION public.payments_purchase_checkout(p_organization_id bigint, p_purpose_code character varying, p_total_amount numeric, p_use_wallet_first boolean DEFAULT true, p_allow_mixed_payment boolean DEFAULT true, p_provider character varying DEFAULT NULL::character varying, p_provider_reference character varying DEFAULT NULL::character varying, p_idempotency_key character varying DEFAULT NULL::character varying, p_description text DEFAULT NULL::text, p_created_by_user_id integer DEFAULT NULL::integer, p_metadata jsonb DEFAULT NULL::jsonb)
                 RETURNS TABLE(operation_id bigint, payment_mode character varying, wallet_used_amount numeric, gateway_charged_amount numeric, wallet_balance_after numeric)
                 LANGUAGE plpgsql
                AS $function$
                DECLARE
                    v_operation_id BIGINT;
                    v_existing_status VARCHAR(12);
                    v_wallet_balance NUMERIC(16,2);
                    v_wallet_use NUMERIC(16,2);
                    v_gateway_charge NUMERIC(16,2);
                    v_mode VARCHAR(12);
                BEGIN
                    IF p_total_amount IS NULL OR p_total_amount <= 0 THEN
                        RAISE EXCEPTION 'Checkout amount must be greater than zero';
                    END IF;

                    IF COALESCE(trim(p_purpose_code), '') = '' THEN
                        RAISE EXCEPTION 'Purpose code is required';
                    END IF;

                    IF p_idempotency_key IS NOT NULL THEN
                        SELECT id, status
                          INTO v_operation_id, v_existing_status
                          FROM public.payments_operation
                         WHERE organization_id = p_organization_id
                           AND idempotency_key = p_idempotency_key
                         LIMIT 1;

                        IF v_operation_id IS NOT NULL THEN
                            IF v_existing_status <> 'completed' THEN
                                RAISE EXCEPTION 'Existing operation with same idempotency key is not completed';
                            END IF;

                            RETURN QUERY
                            SELECT
                                po.id,
                                po.payment_mode,
                                po.wallet_used_amount,
                                po.gateway_charged_amount,
                                public.payments_get_wallet_balance(p_organization_id)
                            FROM public.payments_operation po
                            WHERE po.id = v_operation_id;
                            RETURN;
                        END IF;
                    END IF;

                    v_wallet_balance := public.payments_get_wallet_balance(p_organization_id);
                    IF p_use_wallet_first THEN
                        v_wallet_use := LEAST(v_wallet_balance, p_total_amount);
                    ELSE
                        v_wallet_use := 0;
                    END IF;
                    v_gateway_charge := p_total_amount - v_wallet_use;

                    IF v_wallet_use > 0 AND v_gateway_charge > 0 AND NOT p_allow_mixed_payment THEN
                        v_wallet_use := 0;
                        v_gateway_charge := p_total_amount;
                    END IF;

                    IF v_gateway_charge > 0 THEN
                        IF COALESCE(trim(p_provider), '') = '' THEN
                            RAISE EXCEPTION 'Provider is required when gateway charge is needed';
                        END IF;
                        IF COALESCE(trim(p_provider_reference), '') = '' THEN
                            RAISE EXCEPTION 'Provider reference is required when gateway charge is needed';
                        END IF;
                    END IF;

                    IF v_gateway_charge = 0 THEN
                        v_mode := 'wallet';
                    ELSIF v_wallet_use = 0 THEN
                        v_mode := 'gateway';
                    ELSE
                        v_mode := 'mixed';
                    END IF;

                    INSERT INTO public.payments_operation(
                        organization_id, idempotency_key, operation_type, purpose_code, payment_mode, provider, provider_reference,
                        requested_amount, wallet_used_amount, gateway_charged_amount, final_debit_amount, status,
                        description, metadata, created_by_user_id
                    )
                    VALUES (
                        p_organization_id, p_idempotency_key, 'purchase', p_purpose_code, v_mode,
                        CASE WHEN v_gateway_charge > 0 THEN lower(trim(p_provider)) ELSE NULL END,
                        CASE WHEN v_gateway_charge > 0 THEN trim(p_provider_reference) ELSE NULL END,
                        p_total_amount, v_wallet_use, v_gateway_charge, p_total_amount, 'pending',
                        p_description, p_metadata, p_created_by_user_id
                    )
                    RETURNING id INTO v_operation_id;

                    IF v_gateway_charge > 0 THEN
                        PERFORM 1
                          FROM public.payments_apply_wallet_entry(
                                p_organization_id, v_operation_id, 'credit', 'topup', 'gateway_topup_for_checkout',
                                v_gateway_charge, 'Gateway topup for checkout', p_created_by_user_id, p_metadata
                          );
                    END IF;

                    SELECT pwe.balance_after
                      INTO wallet_balance_after
                      FROM public.payments_apply_wallet_entry(
                            p_organization_id, v_operation_id, 'debit', 'spend', p_purpose_code, p_total_amount,
                            p_description, p_created_by_user_id, p_metadata
                      ) AS pwe;

                    UPDATE public.payments_operation
                       SET status = 'completed',
                           updated_at = NOW()
                     WHERE id = v_operation_id;

                    operation_id := v_operation_id;
                    payment_mode := v_mode;
                    wallet_used_amount := v_wallet_use;
                    gateway_charged_amount := v_gateway_charge;
                    RETURN NEXT;
                END;
                $function$;

                CREATE OR REPLACE VIEW public.payments_wallet_statement AS
                 SELECT e.id AS entry_id,
                    e.organization_id,
                    w.currency_code,
                    e.operation_id,
                    o.operation_uuid,
                    o.operation_type,
                    o.purpose_code,
                    o.payment_mode,
                    o.provider,
                    o.provider_reference,
                    e.entry_side,
                    e.entry_kind,
                    e.source_code,
                    e.amount,
                    e.balance_before,
                    e.balance_after,
                    e.description,
                    e.created_at
                   FROM payments_wallet_entry e
                     JOIN payments_wallet w ON w.id = e.wallet_id
                     JOIN payments_operation o ON o.id = e.operation_id;

                CREATE OR REPLACE VIEW public.payments_wallet_daily_summary AS
                 SELECT organization_id,
                    (created_at AT TIME ZONE 'Asia/Tehran')::date AS local_day,
                    sum(CASE WHEN entry_side = 'credit' THEN amount ELSE 0::numeric END) AS total_credit_amount,
                    sum(CASE WHEN entry_side = 'debit' THEN amount ELSE 0::numeric END) AS total_debit_amount,
                    count(*) AS entry_count
                   FROM payments_wallet_entry e
                  GROUP BY organization_id, ((created_at AT TIME ZONE 'Asia/Tehran')::date);

                DROP TRIGGER IF EXISTS payments_wallet_set_updated_at_bu ON public.payments_wallet;
                CREATE TRIGGER payments_wallet_set_updated_at_bu BEFORE UPDATE ON payments_wallet FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();

                DROP TRIGGER IF EXISTS payments_operation_set_updated_at_bu ON public.payments_operation;
                CREATE TRIGGER payments_operation_set_updated_at_bu BEFORE UPDATE ON payments_operation FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();

                DROP TRIGGER IF EXISTS payments_wallet_entry_prevent_update_bu ON public.payments_wallet_entry;
                CREATE TRIGGER payments_wallet_entry_prevent_update_bu BEFORE UPDATE ON payments_wallet_entry FOR EACH ROW EXECUTE FUNCTION payments_wallet_entry_immutable();

                DROP TRIGGER IF EXISTS payments_wallet_entry_prevent_delete_bd ON public.payments_wallet_entry;
                CREATE TRIGGER payments_wallet_entry_prevent_delete_bd BEFORE DELETE ON payments_wallet_entry FOR EACH ROW EXECUTE FUNCTION payments_wallet_entry_immutable();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP VIEW IF EXISTS public.payments_wallet_daily_summary;
                DROP VIEW IF EXISTS public.payments_wallet_statement;

                DROP TRIGGER IF EXISTS payments_wallet_entry_prevent_delete_bd ON public.payments_wallet_entry;
                DROP TRIGGER IF EXISTS payments_wallet_entry_prevent_update_bu ON public.payments_wallet_entry;
                DROP TRIGGER IF EXISTS payments_operation_set_updated_at_bu ON public.payments_operation;
                DROP TRIGGER IF EXISTS payments_wallet_set_updated_at_bu ON public.payments_wallet;

                DROP FUNCTION IF EXISTS public.payments_purchase_checkout(bigint, character varying, numeric, boolean, boolean, character varying, character varying, character varying, text, integer, jsonb);
                DROP FUNCTION IF EXISTS public.payments_credit_wallet_refund(bigint, numeric, character varying, character varying, text, integer, jsonb);
                DROP FUNCTION IF EXISTS public.payments_credit_wallet_gateway(bigint, numeric, character varying, character varying, character varying, character varying, text, integer, jsonb);
                DROP FUNCTION IF EXISTS public.payments_apply_wallet_entry(bigint, bigint, character varying, character varying, character varying, numeric, text, integer, jsonb);
                DROP FUNCTION IF EXISTS public.payments_get_or_create_wallet(bigint);
                DROP FUNCTION IF EXISTS public.payments_get_wallet_balance(bigint);
                DROP FUNCTION IF EXISTS public.payments_wallet_entry_immutable();
                """);

            migrationBuilder.DropTable(
                name: "payments_operation");

            migrationBuilder.DropTable(
                name: "payments_wallet");

            migrationBuilder.DropTable(
                name: "payments_wallet_entry");
        }
    }
}
