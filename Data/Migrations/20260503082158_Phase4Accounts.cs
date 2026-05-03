using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase4Accounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts_membership",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    primary_group_id = table.Column<int>(type: "integer", nullable: true),
                    is_owner = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts_membership", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts_membership_groups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    membership_id = table.Column<long>(type: "bigint", nullable: false),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts_membership_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts_membership_permissions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    membership_id = table.Column<long>(type: "bigint", nullable: false),
                    permission_id = table.Column<int>(type: "integer", nullable: false),
                    is_granted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts_membership_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts_organization",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    owner_user_id = table.Column<int>(type: "integer", nullable: true),
                    tax_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    national_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    province_id = table.Column<int>(type: "integer", nullable: true),
                    county_id = table.Column<int>(type: "integer", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    logo_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts_organization", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts_organization_security_setting",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<long>(type: "bigint", nullable: false),
                    feature_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts_organization_security_setting", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts_security_feature",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    default_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts_security_feature", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts_user_oauth_identity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    provider = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    provider_subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    provider_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    provider_email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    provider_display_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    provider_picture_url = table.Column<string>(type: "text", nullable: true),
                    linked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_login_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts_user_oauth_identity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts_user_security_setting",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<long>(type: "bigint", nullable: true),
                    feature_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts_user_security_setting", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts_user_session",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    session_public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<long>(type: "bigint", nullable: true),
                    session_secret_hash = table.Column<string>(type: "char(64)", nullable: false),
                    auth_method = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    device_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    device_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    os_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    browser_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_seen_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoke_reason = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts_user_session", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts_user_totp_factor",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<long>(type: "bigint", nullable: true),
                    issuer = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    account_label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    secret_encrypted = table.Column<byte[]>(type: "bytea", nullable: false),
                    algorithm = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    digits = table.Column<short>(type: "smallint", nullable: false),
                    period_seconds = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    enabled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    disabled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_used_counter = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts_user_totp_factor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts_user_verification",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    channel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    purpose = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    destination = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    otp_code_hash = table.Column<string>(type: "char(64)", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    attempt_count = table.Column<short>(type: "smallint", nullable: false),
                    max_attempts = table.Column<short>(type: "smallint", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    verified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts_user_verification", x => x.id);
                });

            migrationBuilder.Sql(
                """
                CREATE EXTENSION IF NOT EXISTS pgcrypto;

                CREATE OR REPLACE FUNCTION public.accounts_hash_text(p_text text)
                 RETURNS character
                 LANGUAGE sql
                 IMMUTABLE STRICT
                AS $function$
                    SELECT encode(digest(p_text, 'sha256'), 'hex')::char(64)
                $function$;

                CREATE OR REPLACE FUNCTION public.accounts_hash_otp_code(p_code text)
                 RETURNS character
                 LANGUAGE sql
                 IMMUTABLE STRICT
                AS $function$
                    SELECT encode(digest(p_code, 'sha256'), 'hex')::char(64)
                $function$;

                CREATE OR REPLACE FUNCTION public.accounts_organization_location_validate()
                 RETURNS trigger
                 LANGUAGE plpgsql
                AS $function$
                DECLARE
                    v_county_province_id INTEGER;
                BEGIN
                    NEW.address := NULLIF(trim(NEW.address), '');
                    NEW.logo_url := NULLIF(trim(NEW.logo_url), '');
                    NEW.postal_code := NULLIF(trim(NEW.postal_code), '');

                    IF NEW.county_id IS NOT NULL THEN
                        SELECT province_id
                        INTO v_county_province_id
                        FROM public.core_county
                        WHERE id = NEW.county_id;

                        IF NOT FOUND THEN
                            RAISE EXCEPTION 'County % not found.', NEW.county_id;
                        END IF;

                        IF NEW.province_id IS NULL THEN
                            NEW.province_id := v_county_province_id;
                        ELSIF NEW.province_id <> v_county_province_id THEN
                            RAISE EXCEPTION 'county_id % does not belong to province_id %.', NEW.county_id, NEW.province_id;
                        END IF;
                    END IF;

                    RETURN NEW;
                END;
                $function$;

                CREATE OR REPLACE FUNCTION public.accounts_user_security_setting_validate()
                 RETURNS trigger
                 LANGUAGE plpgsql
                AS $function$
                BEGIN
                    IF NEW.organization_id IS NOT NULL THEN
                        IF NOT EXISTS (
                            SELECT 1
                            FROM public.accounts_membership m
                            WHERE m.organization_id = NEW.organization_id
                              AND m.user_id = NEW.user_id
                              AND m.is_active = TRUE
                        ) THEN
                            RAISE EXCEPTION 'User % has no active membership in organization %.',
                                NEW.user_id, NEW.organization_id;
                        END IF;
                    END IF;

                    NEW.updated_at := NOW();
                    RETURN NEW;
                END;
                $function$;

                CREATE OR REPLACE FUNCTION public.accounts_user_verification_validate()
                 RETURNS trigger
                 LANGUAGE plpgsql
                AS $function$
                BEGIN
                    IF NEW.channel = 'phone' THEN
                        IF NEW.destination !~ '^[0-9]+$' THEN
                            RAISE EXCEPTION 'Phone destination must contain only English digits.';
                        END IF;
                    ELSIF NEW.channel = 'email' THEN
                        IF NEW.destination !~* '^[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}$' THEN
                            RAISE EXCEPTION 'Email destination format is invalid.';
                        END IF;
                    END IF;

                    IF NEW.expires_at <= NEW.created_at THEN
                        RAISE EXCEPTION 'expires_at must be greater than created_at.';
                    END IF;

                    NEW.updated_at := NOW();
                    RETURN NEW;
                END;
                $function$;

                CREATE OR REPLACE FUNCTION public.accounts_user_session_validate()
                 RETURNS trigger
                 LANGUAGE plpgsql
                AS $function$
                BEGIN
                    IF NEW.issued_at IS NULL THEN
                        NEW.issued_at := NOW();
                    END IF;

                    IF NEW.last_seen_at IS NULL THEN
                        NEW.last_seen_at := NEW.issued_at;
                    END IF;

                    IF NEW.last_seen_at < NEW.issued_at THEN
                        RAISE EXCEPTION 'last_seen_at cannot be earlier than issued_at.';
                    END IF;

                    IF NEW.revoked_at IS NOT NULL AND NEW.revoked_at < NEW.issued_at THEN
                        RAISE EXCEPTION 'revoked_at cannot be earlier than issued_at.';
                    END IF;

                    NEW.updated_at := NOW();
                    RETURN NEW;
                END;
                $function$;

                CREATE OR REPLACE FUNCTION public.accounts_user_oauth_identity_validate()
                 RETURNS trigger
                 LANGUAGE plpgsql
                AS $function$
                BEGIN
                    NEW.provider := lower(trim(NEW.provider));
                    NEW.provider_subject := trim(NEW.provider_subject);
                    NEW.provider_email := NULLIF(lower(trim(NEW.provider_email)), '');
                    NEW.provider_display_name := NULLIF(trim(NEW.provider_display_name), '');
                    NEW.provider_picture_url := NULLIF(trim(NEW.provider_picture_url), '');

                    IF NEW.provider <> 'google' THEN
                        RAISE EXCEPTION 'Only google provider is allowed in current phase.';
                    END IF;

                    IF NEW.provider_subject IS NULL OR length(NEW.provider_subject) = 0 THEN
                        RAISE EXCEPTION 'provider_subject (google sub) is required.';
                    END IF;

                    IF NEW.provider_email IS NOT NULL
                       AND NEW.provider_email !~* '^[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}$' THEN
                        RAISE EXCEPTION 'provider_email format is invalid.';
                    END IF;

                    IF NEW.provider_email_verified = TRUE AND NEW.provider_email IS NULL THEN
                        RAISE EXCEPTION 'provider_email_verified=true requires provider_email.';
                    END IF;

                    NEW.updated_at := NOW();
                    RETURN NEW;
                END;
                $function$;

                DROP TRIGGER IF EXISTS accounts_membership_set_updated_at_bu ON public.accounts_membership;
                CREATE TRIGGER accounts_membership_set_updated_at_bu BEFORE UPDATE ON accounts_membership FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();

                DROP TRIGGER IF EXISTS accounts_organization_location_validate_biu ON public.accounts_organization;
                CREATE TRIGGER accounts_organization_location_validate_biu BEFORE INSERT OR UPDATE ON accounts_organization FOR EACH ROW EXECUTE FUNCTION accounts_organization_location_validate();
                DROP TRIGGER IF EXISTS accounts_organization_set_updated_at_bu ON public.accounts_organization;
                CREATE TRIGGER accounts_organization_set_updated_at_bu BEFORE UPDATE ON accounts_organization FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();

                DROP TRIGGER IF EXISTS accounts_organization_security_setting_set_updated_at_bu ON public.accounts_organization_security_setting;
                CREATE TRIGGER accounts_organization_security_setting_set_updated_at_bu BEFORE UPDATE ON accounts_organization_security_setting FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();

                DROP TRIGGER IF EXISTS accounts_security_feature_set_updated_at_bu ON public.accounts_security_feature;
                CREATE TRIGGER accounts_security_feature_set_updated_at_bu BEFORE UPDATE ON accounts_security_feature FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();

                DROP TRIGGER IF EXISTS accounts_user_oauth_identity_validate_biu ON public.accounts_user_oauth_identity;
                CREATE TRIGGER accounts_user_oauth_identity_validate_biu BEFORE INSERT OR UPDATE ON accounts_user_oauth_identity FOR EACH ROW EXECUTE FUNCTION accounts_user_oauth_identity_validate();
                DROP TRIGGER IF EXISTS accounts_user_oauth_identity_set_updated_at_bu ON public.accounts_user_oauth_identity;
                CREATE TRIGGER accounts_user_oauth_identity_set_updated_at_bu BEFORE UPDATE ON accounts_user_oauth_identity FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();

                DROP TRIGGER IF EXISTS accounts_user_security_setting_validate_biu ON public.accounts_user_security_setting;
                CREATE TRIGGER accounts_user_security_setting_validate_biu BEFORE INSERT OR UPDATE ON accounts_user_security_setting FOR EACH ROW EXECUTE FUNCTION accounts_user_security_setting_validate();
                DROP TRIGGER IF EXISTS accounts_user_security_setting_set_updated_at_bu ON public.accounts_user_security_setting;
                CREATE TRIGGER accounts_user_security_setting_set_updated_at_bu BEFORE UPDATE ON accounts_user_security_setting FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();

                DROP TRIGGER IF EXISTS accounts_user_session_validate_biu ON public.accounts_user_session;
                CREATE TRIGGER accounts_user_session_validate_biu BEFORE INSERT OR UPDATE ON accounts_user_session FOR EACH ROW EXECUTE FUNCTION accounts_user_session_validate();

                DROP TRIGGER IF EXISTS accounts_user_totp_factor_set_updated_at_bu ON public.accounts_user_totp_factor;
                CREATE TRIGGER accounts_user_totp_factor_set_updated_at_bu BEFORE UPDATE ON accounts_user_totp_factor FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();

                DROP TRIGGER IF EXISTS accounts_user_verification_validate_biu ON public.accounts_user_verification;
                CREATE TRIGGER accounts_user_verification_validate_biu BEFORE INSERT OR UPDATE ON accounts_user_verification FOR EACH ROW EXECUTE FUNCTION accounts_user_verification_validate();
                DROP TRIGGER IF EXISTS accounts_user_verification_set_updated_at_bu ON public.accounts_user_verification;
                CREATE TRIGGER accounts_user_verification_set_updated_at_bu BEFORE UPDATE ON accounts_user_verification FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS accounts_user_verification_set_updated_at_bu ON public.accounts_user_verification;
                DROP TRIGGER IF EXISTS accounts_user_verification_validate_biu ON public.accounts_user_verification;
                DROP TRIGGER IF EXISTS accounts_user_totp_factor_set_updated_at_bu ON public.accounts_user_totp_factor;
                DROP TRIGGER IF EXISTS accounts_user_session_validate_biu ON public.accounts_user_session;
                DROP TRIGGER IF EXISTS accounts_user_security_setting_set_updated_at_bu ON public.accounts_user_security_setting;
                DROP TRIGGER IF EXISTS accounts_user_security_setting_validate_biu ON public.accounts_user_security_setting;
                DROP TRIGGER IF EXISTS accounts_user_oauth_identity_set_updated_at_bu ON public.accounts_user_oauth_identity;
                DROP TRIGGER IF EXISTS accounts_user_oauth_identity_validate_biu ON public.accounts_user_oauth_identity;
                DROP TRIGGER IF EXISTS accounts_security_feature_set_updated_at_bu ON public.accounts_security_feature;
                DROP TRIGGER IF EXISTS accounts_organization_security_setting_set_updated_at_bu ON public.accounts_organization_security_setting;
                DROP TRIGGER IF EXISTS accounts_organization_set_updated_at_bu ON public.accounts_organization;
                DROP TRIGGER IF EXISTS accounts_organization_location_validate_biu ON public.accounts_organization;
                DROP TRIGGER IF EXISTS accounts_membership_set_updated_at_bu ON public.accounts_membership;

                DROP FUNCTION IF EXISTS public.accounts_user_oauth_identity_validate();
                DROP FUNCTION IF EXISTS public.accounts_user_session_validate();
                DROP FUNCTION IF EXISTS public.accounts_user_verification_validate();
                DROP FUNCTION IF EXISTS public.accounts_user_security_setting_validate();
                DROP FUNCTION IF EXISTS public.accounts_organization_location_validate();
                DROP FUNCTION IF EXISTS public.accounts_hash_otp_code(text);
                DROP FUNCTION IF EXISTS public.accounts_hash_text(text);
                """);

            migrationBuilder.DropTable(
                name: "accounts_membership");

            migrationBuilder.DropTable(
                name: "accounts_membership_groups");

            migrationBuilder.DropTable(
                name: "accounts_membership_permissions");

            migrationBuilder.DropTable(
                name: "accounts_organization");

            migrationBuilder.DropTable(
                name: "accounts_organization_security_setting");

            migrationBuilder.DropTable(
                name: "accounts_security_feature");

            migrationBuilder.DropTable(
                name: "accounts_user_oauth_identity");

            migrationBuilder.DropTable(
                name: "accounts_user_security_setting");

            migrationBuilder.DropTable(
                name: "accounts_user_session");

            migrationBuilder.DropTable(
                name: "accounts_user_totp_factor");

            migrationBuilder.DropTable(
                name: "accounts_user_verification");
        }
    }
}
