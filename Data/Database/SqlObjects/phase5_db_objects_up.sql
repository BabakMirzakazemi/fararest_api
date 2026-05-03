CREATE SEQUENCE IF NOT EXISTS public.crm_customer_no_seq START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE IF NOT EXISTS public.menu_entity_code_seq START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE IF NOT EXISTS public.support_ticket_no_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE FUNCTION public.digital_menu_generate_token(p_length integer DEFAULT 8)
 RETURNS character varying
 LANGUAGE plpgsql
AS $function$
DECLARE
    chars constant text := 'abcdefghijklmnopqrstuvwxyz0123456789';
    result text := '';
    i int;
BEGIN
    IF p_length IS NULL OR p_length < 4 THEN
        p_length := 8;
    END IF;
    FOR i IN 1..p_length LOOP
        result := result || substr(chars, 1 + floor(random() * length(chars))::int, 1);
    END LOOP;
    RETURN result::varchar;
END;
$function$;

CREATE OR REPLACE FUNCTION public.crm_customer_ledger_after_change()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN
    IF TG_OP = 'DELETE' THEN
        PERFORM public.crm_refresh_customer_balance(OLD.customer_id);
        RETURN OLD;
    END IF;

    PERFORM public.crm_refresh_customer_balance(NEW.customer_id);
    IF TG_OP = 'UPDATE' AND OLD.customer_id <> NEW.customer_id THEN
        PERFORM public.crm_refresh_customer_balance(OLD.customer_id);
    END IF;
    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.crm_customer_ledger_validate()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_allow_credit BOOLEAN;
    v_credit_limit NUMERIC(14,2);
    v_current_net NUMERIC(14,2);
    v_old_effect NUMERIC(14,2);
    v_new_effect NUMERIC(14,2);
    v_projected NUMERIC(14,2);
BEGIN
    NEW.note := NULLIF(trim(NEW.note), '');

    SELECT allow_credit, credit_limit_amount, net_balance_amount
    INTO v_allow_credit, v_credit_limit, v_current_net
    FROM public.crm_customer
    WHERE id = NEW.customer_id
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Customer % not found.', NEW.customer_id;
    END IF;

    v_new_effect := CASE WHEN NEW.entry_type = 'debit' THEN NEW.amount ELSE -NEW.amount END;
    v_old_effect := 0;
    IF TG_OP = 'UPDATE' THEN
        v_old_effect := CASE WHEN OLD.entry_type = 'debit' THEN OLD.amount ELSE -OLD.amount END;
    END IF;

    v_projected := v_current_net - v_old_effect + v_new_effect;

    IF v_projected > 0 THEN
        IF v_allow_credit = FALSE THEN
            RAISE EXCEPTION 'Customer % is not allowed for credit installment/debt.', NEW.customer_id;
        END IF;
        IF v_projected > v_credit_limit THEN
            RAISE EXCEPTION 'Projected debt %.2f exceeds customer credit limit %.2f.', v_projected, v_credit_limit;
        END IF;
    END IF;

    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.crm_customer_validate()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN
    NEW.customer_no := upper(trim(COALESCE(NEW.customer_no, '')));
    IF NEW.customer_no = '' THEN
        NEW.customer_no := public.crm_generate_customer_no();
    END IF;

    NEW.full_name := trim(NEW.full_name);
    NEW.phone := trim(NEW.phone);
    NEW.email := lower(trim(NEW.email));
    NEW.address := trim(NEW.address);

    IF NEW.allow_credit = FALSE THEN
        NEW.credit_limit_amount := 0;
    END IF;

    NEW.updated_at := NOW();
    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.crm_discount_coupon_validate()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN
    NEW.code := upper(trim(NEW.code));
    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.crm_generate_customer_no()
 RETURNS character varying
 LANGUAGE sql
AS $function$
    SELECT ('CUS' || lpad(nextval('public.crm_customer_no_seq')::TEXT, 8, '0'))::VARCHAR(24)
$function$;

CREATE OR REPLACE FUNCTION public.crm_lookup_customer_by_phone(p_organization_id bigint, p_phone character varying)
 RETURNS TABLE(customer_id bigint, customer_no character varying, full_name character varying, phone character varying, email character varying, address text, payment_behavior character varying, balance_status character varying, net_balance_amount numeric, allow_credit boolean, credit_limit_amount numeric, interests text, last_note text, last_note_type character varying)
 LANGUAGE sql
 STABLE
AS $function$
    SELECT
        c.id,
        c.customer_no,
        c.full_name,
        c.phone,
        c.email,
        c.address,
        c.payment_behavior,
        c.balance_status,
        c.net_balance_amount,
        c.allow_credit,
        c.credit_limit_amount,
        (
            SELECT string_agg(t.name, ', ' ORDER BY t.name)
            FROM public.crm_customer_interest ci
            JOIN public.crm_interest_tag t ON t.id = ci.interest_tag_id
            WHERE ci.customer_id = c.id
        ) AS interests,
        (
            SELECT n.body
            FROM public.crm_customer_note n
            WHERE n.customer_id = c.id
            ORDER BY n.created_at DESC
            LIMIT 1
        ) AS last_note,
        (
            SELECT n.note_type
            FROM public.crm_customer_note n
            WHERE n.customer_id = c.id
            ORDER BY n.created_at DESC
            LIMIT 1
        ) AS last_note_type
    FROM public.crm_customer c
    WHERE c.organization_id = p_organization_id
      AND c.phone = trim(p_phone)
      AND c.is_active = TRUE
    ORDER BY c.id
    LIMIT 1
$function$;

CREATE OR REPLACE FUNCTION public.crm_refresh_customer_balance(p_customer_id bigint)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_debit NUMERIC(14,2);
    v_credit NUMERIC(14,2);
    v_net NUMERIC(14,2);
BEGIN
    SELECT
        COALESCE(SUM(CASE WHEN entry_type = 'debit' THEN amount ELSE 0 END), 0),
        COALESCE(SUM(CASE WHEN entry_type = 'credit' THEN amount ELSE 0 END), 0)
    INTO v_debit, v_credit
    FROM public.crm_customer_ledger
    WHERE customer_id = p_customer_id;

    v_net := v_debit - v_credit;

    UPDATE public.crm_customer
    SET total_debit_amount = v_debit,
        total_credit_amount = v_credit,
        net_balance_amount = v_net,
        balance_status = CASE
            WHEN v_net > 0 THEN 'debtor'
            WHEN v_net < 0 THEN 'creditor'
            ELSE 'settled'
        END,
        updated_at = NOW()
    WHERE id = p_customer_id;
END;
$function$;

CREATE OR REPLACE FUNCTION public.crm_suggest_near_expiry_discounts(p_organization_id bigint, p_within_days integer DEFAULT 7)
 RETURNS TABLE(ingredient_id bigint, ingredient_code character varying, ingredient_name character varying, expiration_date date, days_to_expiry integer, suggested_discount_percent numeric)
 LANGUAGE sql
 STABLE
AS $function$
    SELECT
        i.id,
        i.code,
        i.name,
        i.expiration_date,
        (i.expiration_date - CURRENT_DATE)::INTEGER AS days_to_expiry,
        CASE
            WHEN (i.expiration_date - CURRENT_DATE) <= 1 THEN 40
            WHEN (i.expiration_date - CURRENT_DATE) <= 3 THEN 25
            ELSE 15
        END::NUMERIC(5,2) AS suggested_discount_percent
    FROM public.menu_ingredient i
    WHERE i.organization_id = p_organization_id
      AND i.is_active = TRUE
      AND i.is_sellable = TRUE
      AND i.expiration_date IS NOT NULL
      AND i.expiration_date >= CURRENT_DATE
      AND i.expiration_date <= CURRENT_DATE + make_interval(days => GREATEST(1, p_within_days))
    ORDER BY i.expiration_date ASC, i.id
$function$;

CREATE OR REPLACE FUNCTION public.menu_category_validate()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_parent_org BIGINT;
BEGIN
    NEW.name := trim(NEW.name);
    NEW.description := NULLIF(trim(NEW.description), '');
    NEW.updated_at := NOW();

    IF NEW.parent_id IS NOT NULL THEN
        SELECT organization_id
        INTO v_parent_org
        FROM public.menu_category
        WHERE id = NEW.parent_id;

        IF NOT FOUND THEN
            RAISE EXCEPTION 'Parent category % not found.', NEW.parent_id;
        END IF;

        IF v_parent_org <> NEW.organization_id THEN
            RAISE EXCEPTION 'Parent category must belong to same organization.';
        END IF;

        IF NEW.id IS NOT NULL THEN
            IF EXISTS (
                WITH RECURSIVE ancestors AS (
                    SELECT c.id, c.parent_id
                    FROM public.menu_category c
                    WHERE c.id = NEW.parent_id
                    UNION ALL
                    SELECT p.id, p.parent_id
                    FROM public.menu_category p
                    INNER JOIN ancestors a ON a.parent_id = p.id
                )
                SELECT 1
                FROM ancestors
                WHERE id = NEW.id
                LIMIT 1
            ) THEN
                RAISE EXCEPTION 'Category hierarchy cycle detected.';
            END IF;
        END IF;
    END IF;

    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.menu_digital_menu_profile_validate()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN
    NEW.theme_name := trim(COALESCE(NEW.theme_name, 'default'));
    NEW.menu_title := trim(COALESCE(NEW.menu_title, '???? ???????'));
    NEW.menu_description := NULLIF(trim(NEW.menu_description), '');
    NEW.logo_url := NULLIF(trim(NEW.logo_url), '');
    NEW.header_image_url := NULLIF(trim(NEW.header_image_url), '');
    NEW.header_video_url := NULLIF(trim(NEW.header_video_url), '');
    NEW.address := NULLIF(trim(NEW.address), '');
    NEW.phone := NULLIF(trim(NEW.phone), '');
    NEW.social_telegram_url := NULLIF(trim(NEW.social_telegram_url), '');
    NEW.social_instagram_url := NULLIF(trim(NEW.social_instagram_url), '');
    NEW.social_website_url := NULLIF(trim(NEW.social_website_url), '');
    NEW.social_rubika_url := NULLIF(trim(NEW.social_rubika_url), '');
    NEW.social_eitaa_url := NULLIF(trim(NEW.social_eitaa_url), '');
    NEW.social_bale_url := NULLIF(trim(NEW.social_bale_url), '');
    NEW.social_whatsapp_url := NULLIF(trim(NEW.social_whatsapp_url), '');

    IF NEW.logo_url IS NOT NULL AND NEW.logo_url !~* '^(https?://|/).+' THEN
        RAISE EXCEPTION 'logo_url is invalid.';
    END IF;
    IF NEW.header_image_url IS NOT NULL AND NEW.header_image_url !~* '^(https?://|/).+' THEN
        RAISE EXCEPTION 'header_image_url is invalid.';
    END IF;
    IF NEW.header_video_url IS NOT NULL AND NEW.header_video_url !~* '^(https?://|/).+' THEN
        RAISE EXCEPTION 'header_video_url is invalid.';
    END IF;

    NEW.updated_at := NOW();
    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.menu_digital_menu_record_visit(p_digital_menu_id bigint, p_visitor_key text DEFAULT NULL::text, p_ip_address inet DEFAULT NULL::inet, p_user_agent text DEFAULT NULL::text, p_referrer_url text DEFAULT NULL::text)
 RETURNS bigint
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_id BIGINT;
BEGIN
    INSERT INTO public.menu_digital_menu_visit(
        digital_menu_id,
        visitor_key_hash,
        ip_address,
        user_agent,
        referrer_url
    ) VALUES (
        p_digital_menu_id,
        CASE WHEN p_visitor_key IS NULL OR trim(p_visitor_key) = '' THEN NULL
             ELSE public.accounts_hash_text(trim(p_visitor_key))
        END,
        p_ip_address,
        NULLIF(trim(p_user_agent), ''),
        NULLIF(trim(p_referrer_url), '')
    )
    RETURNING id INTO v_id;

    RETURN v_id;
END;
$function$;

CREATE OR REPLACE FUNCTION public.menu_digital_menu_schedule_exception_validate()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN
    NEW.note := NULLIF(trim(NEW.note), '');
    NEW.updated_at := NOW();
    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.menu_digital_menu_validate()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_try INTEGER := 0;
    v_candidate VARCHAR(24);
BEGIN
    NEW.token := lower(trim(COALESCE(NEW.token, '')));
    IF NEW.token = '' THEN
        LOOP
            v_try := v_try + 1;
            v_candidate := public.digital_menu_generate_token(8);
            EXIT WHEN NOT EXISTS (
                SELECT 1
                FROM public.menu_digital_menu m
                WHERE m.token = v_candidate
                  AND (NEW.id IS NULL OR m.id <> NEW.id)
            );
            IF v_try > 25 THEN
                RAISE EXCEPTION 'Could not generate unique digital menu token.';
            END IF;
        END LOOP;
        NEW.token := v_candidate;
    END IF;

    NEW.public_url := trim(COALESCE(NEW.public_url, ''));
    IF NEW.public_url = '' THEN
        NEW.public_url := 'https://fararest.ir/digital-menu/m/' || NEW.token;
    END IF;

    NEW.qr_code_url := NULLIF(trim(NEW.qr_code_url), '');
    NEW.updated_at := NOW();
    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.menu_generate_short_code(p_prefix text DEFAULT 'MN'::text, p_digits integer DEFAULT 6)
 RETURNS character varying
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_prefix TEXT;
    v_digits INTEGER;
    v_seq BIGINT;
BEGIN
    v_prefix := upper(regexp_replace(COALESCE(trim(p_prefix), 'MN'), '[^A-Z0-9_]', '', 'g'));
    IF v_prefix = '' THEN
        v_prefix := 'MN';
    END IF;

    v_digits := COALESCE(p_digits, 6);
    IF v_digits < 4 OR v_digits > 10 THEN
        v_digits := 6;
    END IF;

    v_seq := nextval('public.menu_entity_code_seq');
    RETURN (v_prefix || lpad(v_seq::TEXT, v_digits, '0'))::VARCHAR(20);
END;
$function$;

CREATE OR REPLACE FUNCTION public.menu_ingredient_component_validate()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_prepared_type VARCHAR(12);
    v_prepared_org BIGINT;
    v_component_org BIGINT;
BEGIN
    NEW.notes := NULLIF(trim(NEW.notes), '');

    SELECT ingredient_type, organization_id
    INTO v_prepared_type, v_prepared_org
    FROM public.menu_ingredient
    WHERE id = NEW.prepared_ingredient_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Prepared ingredient % not found.', NEW.prepared_ingredient_id;
    END IF;

    IF v_prepared_type <> 'prepared' THEN
        RAISE EXCEPTION 'Only ingredient_type=prepared can own components.';
    END IF;

    SELECT organization_id
    INTO v_component_org
    FROM public.menu_ingredient
    WHERE id = NEW.component_ingredient_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Component ingredient % not found.', NEW.component_ingredient_id;
    END IF;

    IF v_component_org <> v_prepared_org THEN
        RAISE EXCEPTION 'Prepared and component ingredients must belong to same organization.';
    END IF;

    IF EXISTS (
        WITH RECURSIVE deps AS (
            SELECT NEW.component_ingredient_id AS id, ARRAY[NEW.component_ingredient_id]::BIGINT[] AS path
            UNION ALL
            SELECT mic.component_ingredient_id, deps.path || mic.component_ingredient_id
            FROM public.menu_ingredient_component mic
            INNER JOIN deps ON deps.id = mic.prepared_ingredient_id
            WHERE NOT mic.component_ingredient_id = ANY(deps.path)
        )
        SELECT 1
        FROM deps
        WHERE id = NEW.prepared_ingredient_id
        LIMIT 1
    ) THEN
        RAISE EXCEPTION 'Ingredient component cycle detected.';
    END IF;

    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.menu_ingredient_validate()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN
    NEW.name := trim(NEW.name);
    NEW.description := NULLIF(trim(NEW.description), '');
    NEW.ingredient_type := lower(trim(NEW.ingredient_type));
    NEW.brand_name := NULLIF(trim(NEW.brand_name), '');
    NEW.code := upper(trim(COALESCE(NEW.code, '')));

    IF NEW.code = '' THEN
        NEW.code := CASE NEW.ingredient_type
            WHEN 'raw' THEN public.menu_generate_short_code('RAW')
            WHEN 'branded' THEN public.menu_generate_short_code('BRD')
            WHEN 'prepared' THEN public.menu_generate_short_code('PRP')
            ELSE public.menu_generate_short_code('ING')
        END;
    END IF;

    NEW.updated_at := NOW();
    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.menu_item_ingredient_validate()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_item_org BIGINT;
    v_ing_org BIGINT;
BEGIN
    NEW.notes := NULLIF(trim(NEW.notes), '');

    SELECT organization_id
    INTO v_item_org
    FROM public.menu_item
    WHERE id = NEW.item_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Item % not found.', NEW.item_id;
    END IF;

    SELECT organization_id
    INTO v_ing_org
    FROM public.menu_ingredient
    WHERE id = NEW.ingredient_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Ingredient % not found.', NEW.ingredient_id;
    END IF;

    IF v_item_org <> v_ing_org THEN
        RAISE EXCEPTION 'Item and ingredient must belong to same organization.';
    END IF;

    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.menu_item_validate()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_category_org BIGINT;
BEGIN
    NEW.name := trim(NEW.name);
    NEW.description := NULLIF(trim(NEW.description), '');
    NEW.code := upper(trim(COALESCE(NEW.code, '')));

    IF NEW.code = '' THEN
        NEW.code := public.menu_generate_short_code('ITM');
    END IF;

    SELECT organization_id
    INTO v_category_org
    FROM public.menu_category
    WHERE id = NEW.category_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Category % not found.', NEW.category_id;
    END IF;

    IF v_category_org <> NEW.organization_id THEN
        RAISE EXCEPTION 'Item category must belong to same organization.';
    END IF;

    NEW.updated_at := NOW();
    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.menu_validate_image_urls(p_urls text[])
 RETURNS boolean
 LANGUAGE plpgsql
 IMMUTABLE
AS $function$
DECLARE
    v_url TEXT;
BEGIN
    IF p_urls IS NULL THEN
        RETURN TRUE;
    END IF;

    FOREACH v_url IN ARRAY p_urls
    LOOP
        v_url := trim(COALESCE(v_url, ''));
        IF v_url = '' THEN
            RETURN FALSE;
        END IF;

        IF v_url !~* '^(https?://|/).+' THEN
            RETURN FALSE;
        END IF;
    END LOOP;

    RETURN TRUE;
END;
$function$;

CREATE OR REPLACE FUNCTION public.support_create_ticket(p_organization_id bigint, p_requester_user_id integer, p_subject_code character varying, p_message text, p_attachment_urls text[] DEFAULT NULL::text[])
 RETURNS bigint
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_ticket_id BIGINT;
    v_url TEXT;
BEGIN
    INSERT INTO public.support_ticket (
        organization_id,
        requester_user_id,
        subject_code,
        message,
        status
    ) VALUES (
        p_organization_id,
        p_requester_user_id,
        p_subject_code,
        p_message,
        'open'
    )
    RETURNING id INTO v_ticket_id;

    INSERT INTO public.support_ticket_message (
        ticket_id,
        author_user_id,
        author_type,
        body,
        is_internal
    ) VALUES (
        v_ticket_id,
        p_requester_user_id,
        'requester',
        p_message,
        FALSE
    );

    IF p_attachment_urls IS NOT NULL THEN
        FOREACH v_url IN ARRAY p_attachment_urls
        LOOP
            INSERT INTO public.support_ticket_attachment (
                ticket_id,
                file_url,
                uploaded_by_user_id
            )
            VALUES (
                v_ticket_id,
                v_url,
                p_requester_user_id
            );
        END LOOP;
    END IF;

    RETURN v_ticket_id;
END;
$function$;

CREATE OR REPLACE FUNCTION public.support_generate_ticket_no()
 RETURNS character varying
 LANGUAGE sql
AS $function$
    SELECT ('SUP' || lpad(nextval('public.support_ticket_no_seq')::TEXT, 8, '0'))::VARCHAR(24)
$function$;

CREATE OR REPLACE FUNCTION public.support_is_safe_attachment_url(p_url text)
 RETURNS boolean
 LANGUAGE plpgsql
 IMMUTABLE
AS $function$
DECLARE
    v_url TEXT;
    v_ext TEXT;
BEGIN
    v_url := trim(COALESCE(p_url, ''));
    IF v_url = '' THEN
        RETURN FALSE;
    END IF;

    IF v_url !~* '^(https?://|/).+' THEN
        RETURN FALSE;
    END IF;

    v_ext := lower(regexp_replace(v_url, '^.*\.([a-zA-Z0-9]+)(\?.*)?$', '\1'));
    IF v_ext = v_url THEN
        RETURN FALSE;
    END IF;

    RETURN v_ext IN (
        'png','jpg','jpeg','webp','gif',
        'pdf','txt','csv','xlsx','xls','doc','docx',
        'zip','rar','7z',
        'mp4','mov'
    );
END;
$function$;

CREATE OR REPLACE FUNCTION public.support_ticket_audit_changes()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN
    IF NEW.status IS DISTINCT FROM OLD.status THEN
        INSERT INTO public.support_ticket_event(ticket_id, event_type, old_value, new_value, actor_user_id)
        VALUES (NEW.id, 'status_changed', OLD.status, NEW.status, NULL);
    END IF;

    IF NEW.assigned_to_user_id IS DISTINCT FROM OLD.assigned_to_user_id THEN
        INSERT INTO public.support_ticket_event(ticket_id, event_type, old_value, new_value, actor_user_id)
        VALUES (
            NEW.id,
            'assignment_changed',
            COALESCE(OLD.assigned_to_user_id::TEXT, ''),
            COALESCE(NEW.assigned_to_user_id::TEXT, ''),
            NULL
        );
    END IF;

    IF NEW.priority IS DISTINCT FROM OLD.priority THEN
        INSERT INTO public.support_ticket_event(ticket_id, event_type, old_value, new_value, actor_user_id)
        VALUES (NEW.id, 'priority_changed', OLD.priority, NEW.priority, NULL);
    END IF;

    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.support_ticket_message_after_insert()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN
    UPDATE public.support_ticket t
    SET
        status = CASE
            WHEN NEW.author_type = 'support' THEN 'answered'
            WHEN NEW.author_type = 'requester' AND t.status IN ('answered','closed') THEN 'open'
            ELSE t.status
        END,
        first_response_at = CASE
            WHEN NEW.author_type = 'support' AND t.first_response_at IS NULL THEN NEW.created_at
            ELSE t.first_response_at
        END,
        reopen_count = CASE
            WHEN NEW.author_type = 'requester' AND t.status = 'closed' THEN t.reopen_count + 1
            ELSE t.reopen_count
        END,
        closed_at = CASE
            WHEN NEW.author_type = 'requester' AND t.status = 'closed' THEN NULL
            ELSE t.closed_at
        END,
        resolved_at = CASE
            WHEN NEW.author_type = 'requester' AND t.status = 'closed' THEN NULL
            ELSE t.resolved_at
        END,
        last_activity_at = NEW.created_at,
        updated_at = NOW()
    WHERE t.id = NEW.ticket_id;

    RETURN NEW;
END;
$function$;

CREATE OR REPLACE FUNCTION public.support_ticket_validate()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_old_status TEXT;
BEGIN
    NEW.subject_code := lower(trim(NEW.subject_code));
    NEW.message := trim(NEW.message);
    NEW.status := lower(trim(COALESCE(NEW.status, 'open')));
    NEW.priority := lower(trim(COALESCE(NEW.priority, 'normal')));

    IF NEW.ticket_no IS NULL OR trim(NEW.ticket_no) = '' THEN
        NEW.ticket_no := public.support_generate_ticket_no();
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM public.accounts_membership m
        WHERE m.organization_id = NEW.organization_id
          AND m.user_id = NEW.requester_user_id
          AND m.is_active = TRUE
    ) THEN
        RAISE EXCEPTION 'Requester user % has no active membership in organization %.',
            NEW.requester_user_id, NEW.organization_id;
    END IF;

    IF NEW.assigned_to_user_id IS NOT NULL THEN
        IF NOT EXISTS (
            SELECT 1
            FROM public.accounts_membership m
            WHERE m.organization_id = NEW.organization_id
              AND m.user_id = NEW.assigned_to_user_id
              AND m.is_active = TRUE
        ) THEN
            RAISE EXCEPTION 'Assigned user % has no active membership in organization %.',
                NEW.assigned_to_user_id, NEW.organization_id;
        END IF;
    END IF;

    IF TG_OP = 'UPDATE' THEN
        v_old_status := OLD.status;
        IF v_old_status = 'closed' AND NEW.status = 'open' THEN
            NEW.reopen_count := COALESCE(OLD.reopen_count, 0) + 1;
            NEW.closed_at := NULL;
            NEW.resolved_at := NULL;
        END IF;
    END IF;

    IF NEW.status = 'answered' AND NEW.first_response_at IS NULL THEN
        NEW.first_response_at := NOW();
    END IF;

    IF NEW.status = 'closed' THEN
        NEW.closed_at := COALESCE(NEW.closed_at, NOW());
        NEW.resolved_at := COALESCE(NEW.resolved_at, NEW.closed_at);
    ELSIF NEW.status IN ('open','in_review') THEN
        NEW.closed_at := NULL;
    END IF;

    NEW.last_activity_at := COALESCE(NEW.last_activity_at, NOW());
    NEW.updated_at := NOW();
    RETURN NEW;
END;
$function$;

CREATE TRIGGER core_county_set_updated_at_bu BEFORE UPDATE ON core_county FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER core_province_set_updated_at_bu BEFORE UPDATE ON core_province FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER crm_customer_set_updated_at_bu BEFORE UPDATE ON crm_customer FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER crm_customer_validate_biu BEFORE INSERT OR UPDATE ON crm_customer FOR EACH ROW EXECUTE FUNCTION crm_customer_validate();
CREATE TRIGGER crm_customer_ledger_after_change_aiud AFTER INSERT OR DELETE OR UPDATE ON crm_customer_ledger FOR EACH ROW EXECUTE FUNCTION crm_customer_ledger_after_change();
CREATE TRIGGER crm_customer_ledger_validate_biu BEFORE INSERT OR UPDATE ON crm_customer_ledger FOR EACH ROW EXECUTE FUNCTION crm_customer_ledger_validate();
CREATE TRIGGER crm_customer_loyalty_set_updated_at_bu BEFORE UPDATE ON crm_customer_loyalty FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER crm_discount_campaign_set_updated_at_bu BEFORE UPDATE ON crm_discount_campaign FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER crm_discount_coupon_validate_biu BEFORE INSERT OR UPDATE ON crm_discount_coupon FOR EACH ROW EXECUTE FUNCTION crm_discount_coupon_validate();
CREATE TRIGGER crm_interest_tag_set_updated_at_bu BEFORE UPDATE ON crm_interest_tag FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER crm_loyalty_tier_set_updated_at_bu BEFORE UPDATE ON crm_loyalty_tier FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER crm_organization_phone_set_updated_at_bu BEFORE UPDATE ON crm_organization_phone FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER menu_category_set_updated_at_bu BEFORE UPDATE ON menu_category FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER menu_category_validate_biu BEFORE INSERT OR UPDATE ON menu_category FOR EACH ROW EXECUTE FUNCTION menu_category_validate();
CREATE TRIGGER menu_digital_menu_set_updated_at_bu BEFORE UPDATE ON menu_digital_menu FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER menu_digital_menu_validate_biu BEFORE INSERT OR UPDATE ON menu_digital_menu FOR EACH ROW EXECUTE FUNCTION menu_digital_menu_validate();
CREATE TRIGGER menu_digital_menu_profile_set_updated_at_bu BEFORE UPDATE ON menu_digital_menu_profile FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER menu_digital_menu_profile_validate_biu BEFORE INSERT OR UPDATE ON menu_digital_menu_profile FOR EACH ROW EXECUTE FUNCTION menu_digital_menu_profile_validate();
CREATE TRIGGER menu_digital_menu_schedule_exception_set_updated_at_bu BEFORE UPDATE ON menu_digital_menu_schedule_exception FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER menu_digital_menu_schedule_exception_validate_biu BEFORE INSERT OR UPDATE ON menu_digital_menu_schedule_exception FOR EACH ROW EXECUTE FUNCTION menu_digital_menu_schedule_exception_validate();
CREATE TRIGGER menu_digital_menu_schedule_weekly_set_updated_at_bu BEFORE UPDATE ON menu_digital_menu_schedule_weekly FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER menu_ingredient_set_updated_at_bu BEFORE UPDATE ON menu_ingredient FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER menu_ingredient_validate_biu BEFORE INSERT OR UPDATE ON menu_ingredient FOR EACH ROW EXECUTE FUNCTION menu_ingredient_validate();
CREATE TRIGGER menu_ingredient_component_validate_biu BEFORE INSERT OR UPDATE ON menu_ingredient_component FOR EACH ROW EXECUTE FUNCTION menu_ingredient_component_validate();
CREATE TRIGGER menu_item_set_updated_at_bu BEFORE UPDATE ON menu_item FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER menu_item_validate_biu BEFORE INSERT OR UPDATE ON menu_item FOR EACH ROW EXECUTE FUNCTION menu_item_validate();
CREATE TRIGGER menu_item_ingredient_validate_biu BEFORE INSERT OR UPDATE ON menu_item_ingredient FOR EACH ROW EXECUTE FUNCTION menu_item_ingredient_validate();
CREATE TRIGGER menu_unit_set_updated_at_bu BEFORE UPDATE ON menu_unit FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER support_ticket_audit_changes_au AFTER UPDATE ON support_ticket FOR EACH ROW EXECUTE FUNCTION support_ticket_audit_changes();
CREATE TRIGGER support_ticket_set_updated_at_bu BEFORE UPDATE ON support_ticket FOR EACH ROW EXECUTE FUNCTION set_row_updated_at();
CREATE TRIGGER support_ticket_validate_biu BEFORE INSERT OR UPDATE ON support_ticket FOR EACH ROW EXECUTE FUNCTION support_ticket_validate();
CREATE TRIGGER support_ticket_message_after_insert_ai AFTER INSERT ON support_ticket_message FOR EACH ROW EXECUTE FUNCTION support_ticket_message_after_insert();






