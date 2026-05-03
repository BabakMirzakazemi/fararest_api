CREATE OR REPLACE FUNCTION public.accounts_count_active_sessions(p_user_id integer)
 RETURNS integer
 LANGUAGE sql
 STABLE
AS $function$
    SELECT COUNT(*)::INTEGER
    FROM public.accounts_user_session s
    WHERE s.user_id = p_user_id
      AND s.revoked_at IS NULL
      AND s.expires_at > NOW()
$function$;
CREATE OR REPLACE FUNCTION public.accounts_create_session(p_user_id integer, p_session_secret text, p_device_type character varying DEFAULT 'unknown'::character varying, p_device_name character varying DEFAULT NULL::character varying, p_os_name character varying DEFAULT NULL::character varying, p_browser_name character varying DEFAULT NULL::character varying, p_ip_address inet DEFAULT NULL::inet, p_user_agent text DEFAULT NULL::text, p_ttl_minutes integer DEFAULT 43200, p_auth_method character varying DEFAULT 'password'::character varying, p_organization_id bigint DEFAULT NULL::bigint)
 RETURNS TABLE(session_public_id uuid, expires_at timestamp with time zone)
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_is_active BOOLEAN;
BEGIN
    IF p_ttl_minutes < 5 OR p_ttl_minutes > 43200 THEN
        RAISE EXCEPTION 'Session TTL must be between 5 and 43200 minutes.';
    END IF;

    IF p_session_secret IS NULL OR length(trim(p_session_secret)) < 16 THEN
        RAISE EXCEPTION 'Session secret is too short.';
    END IF;

    SELECT is_active
    INTO v_is_active
    FROM public.auth_user
    WHERE id = p_user_id
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'User % not found.', p_user_id;
    END IF;

    IF v_is_active IS DISTINCT FROM TRUE THEN
        RAISE EXCEPTION 'User % is not active.', p_user_id;
    END IF;

    RETURN QUERY
    INSERT INTO public.accounts_user_session (
        user_id,
        organization_id,
        session_secret_hash,
        auth_method,
        device_type,
        device_name,
        os_name,
        browser_name,
        ip_address,
        user_agent,
        issued_at,
        last_seen_at,
        expires_at
    )
    VALUES (
        p_user_id,
        p_organization_id,
        public.accounts_hash_text(p_session_secret),
        p_auth_method,
        COALESCE(p_device_type, 'unknown'),
        p_device_name,
        p_os_name,
        p_browser_name,
        p_ip_address,
        p_user_agent,
        NOW(),
        NOW(),
        NOW() + make_interval(mins => p_ttl_minutes)
    )
    RETURNING accounts_user_session.session_public_id, accounts_user_session.expires_at;
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_disable_totp_for_user(p_user_id integer)
 RETURNS boolean
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_row_count INTEGER;
BEGIN
    UPDATE public.accounts_user_totp_factor
    SET status = 'disabled',
        disabled_at = NOW(),
        updated_at = NOW()
    WHERE user_id = p_user_id
      AND status IN ('pending','enabled');

    GET DIAGNOSTICS v_row_count = ROW_COUNT;
    RETURN v_row_count = 1;
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_enable_totp_for_user(p_user_id integer, p_secret_raw_hex text, p_crypto_key text, p_issuer character varying DEFAULT 'Fararest'::character varying, p_account_label character varying DEFAULT NULL::character varying, p_algorithm character varying DEFAULT 'SHA1'::character varying, p_digits smallint DEFAULT 6, p_period_seconds integer DEFAULT 30, p_organization_id bigint DEFAULT NULL::bigint)
 RETURNS bigint
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_id BIGINT;
    v_secret_raw BYTEA;
BEGIN
    IF p_secret_raw_hex !~ '^[0-9a-fA-F]{32,128}$' THEN
        RAISE EXCEPTION 'Secret hex must be 16..64 bytes encoded as hex.';
    END IF;

    v_secret_raw := decode(lower(p_secret_raw_hex), 'hex');

    INSERT INTO public.accounts_user_totp_factor (
        user_id, organization_id, issuer, account_label, secret_encrypted,
        algorithm, digits, period_seconds, status, enabled_at, disabled_at, last_used_counter
    )
    VALUES (
        p_user_id,
        p_organization_id,
        COALESCE(NULLIF(trim(p_issuer),''), 'Fararest'),
        p_account_label,
        pgp_sym_encrypt_bytea(v_secret_raw, p_crypto_key),
        upper(p_algorithm),
        p_digits,
        p_period_seconds,
        'pending',
        NULL,
        NULL,
        -1
    )
    ON CONFLICT (user_id)
    DO UPDATE SET
        organization_id = EXCLUDED.organization_id,
        issuer = EXCLUDED.issuer,
        account_label = EXCLUDED.account_label,
        secret_encrypted = EXCLUDED.secret_encrypted,
        algorithm = EXCLUDED.algorithm,
        digits = EXCLUDED.digits,
        period_seconds = EXCLUDED.period_seconds,
        status = 'pending',
        enabled_at = NULL,
        disabled_at = NULL,
        last_used_counter = -1,
        updated_at = NOW()
    RETURNING id INTO v_id;

    RETURN v_id;
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_expire_sessions(p_user_id integer DEFAULT NULL::integer)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_row_count INTEGER;
BEGIN
    UPDATE public.accounts_user_session s
    SET revoked_at = NOW(),
        revoke_reason = COALESCE(s.revoke_reason, 'expired'),
        updated_at = NOW()
    WHERE s.revoked_at IS NULL
      AND s.expires_at <= NOW()
      AND (p_user_id IS NULL OR s.user_id = p_user_id);

    GET DIAGNOSTICS v_row_count = ROW_COUNT;
    RETURN v_row_count;
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_get_user_id_by_google_sub(p_google_sub character varying)
 RETURNS integer
 LANGUAGE sql
 STABLE
AS $function$
    SELECT oi.user_id
    FROM public.accounts_user_oauth_identity oi
    WHERE oi.provider = 'google'
      AND oi.provider_subject = trim(p_google_sub)
    LIMIT 1
$function$;
CREATE OR REPLACE FUNCTION public.accounts_hash_otp_code(p_code text)
 RETURNS character
 LANGUAGE sql
 IMMUTABLE STRICT
AS $function$
    SELECT encode(digest(p_code, 'sha256'), 'hex')::char(64)
$function$;
CREATE OR REPLACE FUNCTION public.accounts_hash_text(p_text text)
 RETURNS character
 LANGUAGE sql
 IMMUTABLE STRICT
AS $function$
    SELECT encode(digest(p_text, 'sha256'), 'hex')::char(64)
$function$;
CREATE OR REPLACE FUNCTION public.accounts_hotp_value(p_secret_raw bytea, p_counter bigint, p_digits smallint, p_algorithm character varying)
 RETURNS integer
 LANGUAGE plpgsql
 IMMUTABLE
AS $function$
DECLARE
    v_counter_hex TEXT;
    v_counter_bytes BYTEA;
    v_hash BYTEA;
    v_offset INTEGER;
    v_binary BIGINT;
    v_mod BIGINT;
BEGIN
    v_counter_hex := lpad(to_hex(p_counter), 16, '0');
    v_counter_bytes := decode(v_counter_hex, 'hex');

    v_hash := hmac(v_counter_bytes, p_secret_raw, lower(p_algorithm));
    v_offset := get_byte(v_hash, length(v_hash) - 1) & 15;

    v_binary :=
        ((get_byte(v_hash, v_offset) & 127)::BIGINT << 24) +
        (get_byte(v_hash, v_offset + 1)::BIGINT << 16) +
        (get_byte(v_hash, v_offset + 2)::BIGINT << 8) +
        (get_byte(v_hash, v_offset + 3)::BIGINT);

    v_mod := power(10::numeric, p_digits)::BIGINT;
    RETURN (v_binary % v_mod)::INTEGER;
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_is_feature_enabled(p_feature_code character varying, p_user_id integer DEFAULT NULL::integer, p_organization_id bigint DEFAULT NULL::bigint)
 RETURNS boolean
 LANGUAGE plpgsql
 STABLE
AS $function$
DECLARE
    v_feature_id BIGINT;
    v_default BOOLEAN;
    v_org_override BOOLEAN;
    v_user_override BOOLEAN;
BEGIN
    SELECT id, default_enabled
    INTO v_feature_id, v_default
    FROM public.accounts_security_feature
    WHERE code = p_feature_code;

    IF v_feature_id IS NULL THEN
        RAISE EXCEPTION 'Unknown security feature code: %', p_feature_code;
    END IF;

    IF p_organization_id IS NOT NULL THEN
        SELECT is_enabled INTO v_org_override
        FROM public.accounts_organization_security_setting
        WHERE organization_id = p_organization_id
          AND feature_id = v_feature_id
        LIMIT 1;
    END IF;

    IF p_user_id IS NOT NULL THEN
        SELECT is_enabled INTO v_user_override
        FROM public.accounts_user_security_setting
        WHERE user_id = p_user_id
          AND feature_id = v_feature_id
          AND (
                (organization_id IS NULL AND p_organization_id IS NULL)
                OR organization_id = p_organization_id
          )
        ORDER BY organization_id NULLS LAST
        LIMIT 1;
    END IF;

    RETURN COALESCE(v_user_override, v_org_override, v_default);
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_issue_signup_otp(p_user_id integer, p_channel character varying, p_code text, p_expire_minutes integer DEFAULT 2)
 RETURNS bigint
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_destination TEXT;
    v_verification_id BIGINT;
BEGIN
    IF p_channel NOT IN ('email','phone') THEN
        RAISE EXCEPTION 'Invalid channel: %', p_channel;
    END IF;

    IF p_code !~ '^[0-9]{4,8}$' THEN
        RAISE EXCEPTION 'OTP code must be 4 to 8 English digits.';
    END IF;

    IF p_expire_minutes < 1 OR p_expire_minutes > 60 THEN
        RAISE EXCEPTION 'Expire minutes must be between 1 and 60.';
    END IF;

    IF p_channel = 'phone' THEN
        SELECT phone INTO v_destination
        FROM public.auth_user
        WHERE id = p_user_id
        FOR UPDATE;

        IF v_destination IS NULL OR v_destination = '' THEN
            RAISE EXCEPTION 'User % has no phone number.', p_user_id;
        END IF;
    ELSE
        SELECT lower(trim(email)) INTO v_destination
        FROM public.auth_user
        WHERE id = p_user_id
        FOR UPDATE;

        IF v_destination IS NULL OR v_destination = '' THEN
            RAISE EXCEPTION 'User % has no email.', p_user_id;
        END IF;
    END IF;

    UPDATE public.accounts_user_verification
    SET status = 'cancelled',
        updated_at = NOW()
    WHERE user_id = p_user_id
      AND channel = p_channel
      AND purpose = 'signup'
      AND status = 'pending';

    INSERT INTO public.accounts_user_verification (
        user_id, channel, purpose, destination, otp_code_hash, status,
        attempt_count, max_attempts, expires_at
    )
    VALUES (
        p_user_id,
        p_channel,
        'signup',
        v_destination,
        public.accounts_hash_otp_code(p_code),
        'pending',
        0,
        5,
        NOW() + make_interval(mins => p_expire_minutes)
    )
    RETURNING id INTO v_verification_id;

    RETURN v_verification_id;
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_link_google_identity(p_user_id integer, p_google_sub character varying, p_google_email character varying DEFAULT NULL::character varying, p_google_email_verified boolean DEFAULT false, p_display_name character varying DEFAULT NULL::character varying, p_picture_url text DEFAULT NULL::text, p_touch_last_login boolean DEFAULT true)
 RETURNS bigint
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_existing_sub VARCHAR(191);
    v_identity_id BIGINT;
    v_identity_user_id INTEGER;
BEGIN
    IF p_google_sub IS NULL OR length(trim(p_google_sub)) = 0 THEN
        RAISE EXCEPTION 'Google subject (sub) is required.';
    END IF;

    PERFORM 1
    FROM public.auth_user
    WHERE id = p_user_id;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'User % not found.', p_user_id;
    END IF;

    -- One google identity per user, and immutable sub for that user.
    SELECT provider_subject
    INTO v_existing_sub
    FROM public.accounts_user_oauth_identity
    WHERE user_id = p_user_id
      AND provider = 'google'
    LIMIT 1;

    IF v_existing_sub IS NOT NULL AND v_existing_sub <> p_google_sub THEN
        RAISE EXCEPTION 'User % already linked to another Google subject.', p_user_id;
    END IF;

    INSERT INTO public.accounts_user_oauth_identity (
        user_id,
        provider,
        provider_subject,
        provider_email,
        provider_email_verified,
        provider_display_name,
        provider_picture_url,
        linked_at,
        last_login_at
    )
    VALUES (
        p_user_id,
        'google',
        trim(p_google_sub),
        NULLIF(lower(trim(p_google_email)), ''),
        COALESCE(p_google_email_verified, FALSE),
        NULLIF(trim(p_display_name), ''),
        NULLIF(trim(p_picture_url), ''),
        NOW(),
        CASE WHEN p_touch_last_login THEN NOW() ELSE NULL END
    )
    ON CONFLICT (provider, provider_subject)
    DO UPDATE SET
        provider_email = EXCLUDED.provider_email,
        provider_email_verified = EXCLUDED.provider_email_verified,
        provider_display_name = EXCLUDED.provider_display_name,
        provider_picture_url = EXCLUDED.provider_picture_url,
        last_login_at = CASE
            WHEN p_touch_last_login THEN NOW()
            ELSE public.accounts_user_oauth_identity.last_login_at
        END,
        updated_at = NOW()
    WHERE public.accounts_user_oauth_identity.user_id = EXCLUDED.user_id
    RETURNING id, user_id INTO v_identity_id, v_identity_user_id;

    IF v_identity_id IS NULL THEN
        SELECT user_id
        INTO v_identity_user_id
        FROM public.accounts_user_oauth_identity
        WHERE provider = 'google'
          AND provider_subject = trim(p_google_sub)
        LIMIT 1;
        RAISE EXCEPTION 'Google subject is already linked to user %.', v_identity_user_id;
    END IF;

    IF v_identity_user_id <> p_user_id THEN
        RAISE EXCEPTION 'Google subject ownership mismatch: expected user %, got %.', p_user_id, v_identity_user_id;
    END IF;

    RETURN v_identity_id;
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_list_active_sessions(p_user_id integer)
 RETURNS TABLE(session_public_id uuid, device_type character varying, device_name character varying, os_name character varying, browser_name character varying, ip_address inet, issued_at timestamp with time zone, last_seen_at timestamp with time zone, expires_at timestamp with time zone, remaining_seconds bigint)
 LANGUAGE sql
 STABLE
AS $function$
    SELECT
        s.session_public_id,
        s.device_type,
        s.device_name,
        s.os_name,
        s.browser_name,
        s.ip_address,
        s.issued_at,
        s.last_seen_at,
        s.expires_at,
        GREATEST(0, EXTRACT(EPOCH FROM (s.expires_at - NOW()))::BIGINT) AS remaining_seconds
    FROM public.accounts_user_session s
    WHERE s.user_id = p_user_id
      AND s.revoked_at IS NULL
      AND s.expires_at > NOW()
    ORDER BY s.last_seen_at DESC;
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
CREATE OR REPLACE FUNCTION public.accounts_revoke_all_sessions(p_user_id integer, p_except_session_public_id uuid DEFAULT NULL::uuid, p_reason character varying DEFAULT 'user_logout_all'::character varying)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_row_count INTEGER;
BEGIN
    UPDATE public.accounts_user_session
    SET revoked_at = NOW(),
        revoke_reason = COALESCE(NULLIF(trim(p_reason), ''), 'user_logout_all'),
        updated_at = NOW()
    WHERE user_id = p_user_id
      AND revoked_at IS NULL
      AND (p_except_session_public_id IS NULL OR session_public_id <> p_except_session_public_id);

    GET DIAGNOSTICS v_row_count = ROW_COUNT;
    RETURN v_row_count;
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_revoke_session(p_user_id integer, p_session_public_id uuid, p_reason character varying DEFAULT 'user_logout'::character varying)
 RETURNS boolean
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_row_count INTEGER;
BEGIN
    UPDATE public.accounts_user_session
    SET revoked_at = NOW(),
        revoke_reason = COALESCE(NULLIF(trim(p_reason), ''), 'user_logout'),
        updated_at = NOW()
    WHERE user_id = p_user_id
      AND session_public_id = p_session_public_id
      AND revoked_at IS NULL;

    GET DIAGNOSTICS v_row_count = ROW_COUNT;
    RETURN v_row_count = 1;
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_set_org_feature(p_organization_id bigint, p_feature_code character varying, p_is_enabled boolean)
 RETURNS bigint
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_feature_id BIGINT;
    v_id BIGINT;
BEGIN
    SELECT id INTO v_feature_id
    FROM public.accounts_security_feature
    WHERE code = p_feature_code;

    IF v_feature_id IS NULL THEN
        RAISE EXCEPTION 'Unknown security feature code: %', p_feature_code;
    END IF;

    INSERT INTO public.accounts_organization_security_setting (
        organization_id, feature_id, is_enabled
    )
    VALUES (p_organization_id, v_feature_id, p_is_enabled)
    ON CONFLICT (organization_id, feature_id)
    DO UPDATE SET
        is_enabled = EXCLUDED.is_enabled,
        updated_at = NOW()
    RETURNING id INTO v_id;

    RETURN v_id;
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_set_user_feature(p_user_id integer, p_feature_code character varying, p_is_enabled boolean, p_organization_id bigint DEFAULT NULL::bigint)
 RETURNS bigint
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_feature_id BIGINT;
    v_id BIGINT;
BEGIN
    SELECT id INTO v_feature_id
    FROM public.accounts_security_feature
    WHERE code = p_feature_code;

    IF v_feature_id IS NULL THEN
        RAISE EXCEPTION 'Unknown security feature code: %', p_feature_code;
    END IF;

    INSERT INTO public.accounts_user_security_setting (
        user_id, organization_id, feature_id, is_enabled
    )
    VALUES (p_user_id, p_organization_id, v_feature_id, p_is_enabled)
    ON CONFLICT (user_id, organization_id, feature_id)
    DO UPDATE SET
        is_enabled = EXCLUDED.is_enabled,
        updated_at = NOW()
    RETURNING id INTO v_id;

    RETURN v_id;
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_touch_session(p_session_public_id uuid, p_session_secret text)
 RETURNS boolean
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_row_count INTEGER;
BEGIN
    UPDATE public.accounts_user_session
    SET last_seen_at = NOW(),
        updated_at = NOW()
    WHERE session_public_id = p_session_public_id
      AND session_secret_hash = public.accounts_hash_text(p_session_secret)
      AND revoked_at IS NULL
      AND expires_at > NOW();

    GET DIAGNOSTICS v_row_count = ROW_COUNT;
    RETURN v_row_count = 1;
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
CREATE OR REPLACE FUNCTION public.accounts_verify_signup_otp(p_user_id integer, p_channel character varying, p_code text)
 RETURNS boolean
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_rec public.accounts_user_verification%ROWTYPE;
    v_hash CHAR(64);
BEGIN
    IF p_channel NOT IN ('email','phone') THEN
        RAISE EXCEPTION 'Invalid channel: %', p_channel;
    END IF;

    IF p_code !~ '^[0-9]{4,8}$' THEN
        RAISE EXCEPTION 'OTP code must be 4 to 8 English digits.';
    END IF;

    SELECT *
    INTO v_rec
    FROM public.accounts_user_verification
    WHERE user_id = p_user_id
      AND channel = p_channel
      AND purpose = 'signup'
      AND status = 'pending'
    ORDER BY created_at DESC
    LIMIT 1
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No pending signup OTP found for user % and channel %.', p_user_id, p_channel;
    END IF;

    IF NOW() > v_rec.expires_at THEN
        UPDATE public.accounts_user_verification
        SET status = 'expired',
            updated_at = NOW()
        WHERE id = v_rec.id;
        RETURN FALSE;
    END IF;

    IF v_rec.attempt_count >= v_rec.max_attempts THEN
        UPDATE public.accounts_user_verification
        SET status = 'failed',
            updated_at = NOW()
        WHERE id = v_rec.id;
        RETURN FALSE;
    END IF;

    v_hash := public.accounts_hash_otp_code(p_code);

    IF v_hash <> v_rec.otp_code_hash THEN
        UPDATE public.accounts_user_verification
        SET attempt_count = attempt_count + 1,
            status = CASE WHEN attempt_count + 1 >= max_attempts THEN 'failed' ELSE 'pending' END,
            updated_at = NOW()
        WHERE id = v_rec.id;
        RETURN FALSE;
    END IF;

    UPDATE public.accounts_user_verification
    SET status = 'verified',
        verified_at = NOW(),
        updated_at = NOW()
    WHERE id = v_rec.id;

    UPDATE public.auth_user
    SET is_active = TRUE
    WHERE id = p_user_id;

    RETURN TRUE;
END;
$function$;
CREATE OR REPLACE FUNCTION public.accounts_verify_totp_code(p_user_id integer, p_code text, p_crypto_key text, p_window_steps integer DEFAULT 1, p_activate_if_pending boolean DEFAULT true)
 RETURNS boolean
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_factor public.accounts_user_totp_factor%ROWTYPE;
    v_secret_raw BYTEA;
    v_current_counter BIGINT;
    v_counter BIGINT;
    v_expected INTEGER;
    v_input INTEGER;
BEGIN
    IF p_code !~ '^[0-9]{6,8}$' THEN
        RAISE EXCEPTION 'Invalid TOTP code format.';
    END IF;

    IF p_window_steps < 0 OR p_window_steps > 3 THEN
        RAISE EXCEPTION 'Window steps must be between 0 and 3.';
    END IF;

    SELECT *
    INTO v_factor
    FROM public.accounts_user_totp_factor
    WHERE user_id = p_user_id
      AND status IN ('pending','enabled')
    FOR UPDATE;

    IF NOT FOUND THEN
        RETURN FALSE;
    END IF;

    v_secret_raw := pgp_sym_decrypt_bytea(v_factor.secret_encrypted, p_crypto_key);
    v_current_counter := floor(extract(epoch FROM NOW()) / v_factor.period_seconds)::BIGINT;
    v_input := p_code::INTEGER;

    FOR v_counter IN (v_current_counter - p_window_steps)..(v_current_counter + p_window_steps) LOOP
        v_expected := public.accounts_hotp_value(v_secret_raw, v_counter, v_factor.digits, v_factor.algorithm);
        IF v_expected = v_input THEN
            IF v_counter <= v_factor.last_used_counter THEN
                RETURN FALSE;
            END IF;

            UPDATE public.accounts_user_totp_factor
            SET last_used_counter = v_counter,
                status = CASE
                            WHEN status = 'pending' AND p_activate_if_pending THEN 'enabled'
                            ELSE status
                         END,
                enabled_at = CASE
                               WHEN enabled_at IS NULL AND p_activate_if_pending THEN NOW()
                               ELSE enabled_at
                             END,
                updated_at = NOW()
            WHERE id = v_factor.id;

            RETURN TRUE;
        END IF;
    END LOOP;

    RETURN FALSE;
END;
$function$;
