CREATE OR REPLACE PROCEDURE buy_item (
        p_store_item_id uuid,
        p_character_id uuid
)
SECURITY DEFINER
AS $$
DECLARE
        client_b balances%ROWTYPE;
        client_s_i store_items%ROWTYPE;
        row_count int;
BEGIN
        SELECT * INTO client_s_i FROM store_items si WHERE si.id = p_store_item_id;
        IF NOT FOUND THEN
            RAISE EXCEPTION 'STORE ITEM NOT FOUND';
        END IF;

        SELECT * INTO client_b FROM balances b WHERE b.character_id = p_character_id AND b.currency_id = client_s_i.currency_id;
        IF NOT FOUND THEN
            RAISE EXCEPTION 'BALANCE NOT FOUND';
        END IF;
        
        IF client_b.amount < client_s_i.price THEN
            RAISE EXCEPTION 'BALANCE NOT ENOUGH';
        END IF;

        -- DELETE FROM STORE ITEMS
        DELETE FROM store_items WHERE id = p_store_item_id;
        GET DIAGNOSTICS row_count = ROW_COUNT;
        IF row_count = 0 THEN
            RAISE EXCEPTION 'CONFLICT';
        END IF;
        
        -- UPDATE BALANCE
        UPDATE balances SET amount = amount - client_s_i.price WHERE id = client_b.id;
        GET DIAGNOSTICS row_count = ROW_COUNT;
        IF row_count = 0 THEN
            RAISE EXCEPTION 'CONFLICT';
        END IF;
        IF (SELECT amount FROM balances WHERE id = client_b.id LIMIT 1) < 0 THEN
            RAISE EXCEPTION 'CONFLICT';
        END IF;
        
        -- INSERT INTO INVENTORY
        INSERT INTO inventory_items values (gen_random_uuid(), client_b.character_id, client_s_i.artifact_id);
END;
$$ LANGUAGE plpgsql;
   
CREATE OR REPLACE PROCEDURE sell_item (
        inventory_item_id uuid,
        price_currency_id uuid,
        price numeric(19, 4)
)
SECURITY DEFINER
AS $$
DECLARE
        client_i_i inventory_items%ROWTYPE;
        client_b balances%ROWTYPE;
        artifact_id uuid;
        row_count int;
BEGIN
        SELECT * INTO client_i_i FROM inventory_items ii WHERE ii.id = inventory_item_id;
        IF NOT FOUND THEN
            RAISE EXCEPTION 'INVENTORY ITEM NOT FOUND';
        END IF;
        
        artifact_id = client_i_i.artifact_id;

        SELECT * INTO client_b FROM balances b WHERE b.character_id = client_i_i.character_id AND b.currency_id = price_currency_id;
        IF NOT FOUND THEN
            RAISE EXCEPTION 'BALANCE NOT FOUND';
        END IF;
        
        -- DELETE FROM INVENTORY ITEM
        DELETE FROM inventory_items WHERE id = inventory_item_id;
        GET DIAGNOSTICS row_count = ROW_COUNT;
        IF row_count = 0 THEN
            RAISE EXCEPTION 'CONFLICT';
        END IF;

        -- UPDATE BALANCE
        UPDATE balances SET amount = amount + price WHERE id = client_b.id;
        GET DIAGNOSTICS row_count = ROW_COUNT;
        IF row_count = 0 THEN
            RAISE EXCEPTION 'CONFLICT';
        END IF;

        -- INSERT INTO STORE ITEMS
        INSERT INTO store_items
        VALUES (gen_random_uuid(), client_b.currency_id, artifact_id, price + price * 0.05);      
        
        COMMIT;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE register_account(
    account_id uuid,
    role_name varchar(32),
    nickname varchar(32),
    password varchar(256),
    email varchar(254),
    register_date date
)
SECURITY DEFINER
AS $$ 
DECLARE
    roleId uuid;
BEGIN 
    SELECT id INTO roleId FROM account_roles ar WHERE lower(role_name) = lower(ar.name);
    IF NOT FOUND THEN
        RAISE EXCEPTION 'ROLE NOT FOUND';
    END IF;
    
    INSERT INTO accounts
    values (account_id, roleId, nickname, password, email, register_date, null);
EXCEPTION
    WHEN unique_violation THEN
        DECLARE
            v_constraint text;
        BEGIN
            GET STACKED DIAGNOSTICS v_constraint = CONSTRAINT_NAME;
            IF v_constraint = 'accounts_nickname_unique' THEN
                RAISE EXCEPTION 'NICKNAME ALREADY EXISTS';
            ELSIF v_constraint = 'accounts_email_unique' THEN
                RAISE EXCEPTION 'EMAIL ALREADY EXISTS';
            END IF;
        END;
        
    WHEN check_violation THEN
        DECLARE
            v_constraint text;
        BEGIN
            GET STACKED DIAGNOSTICS v_constraint = CONSTRAINT_NAME;
            IF v_constraint = 'account_min_len_password' THEN
                RAISE EXCEPTION 'PASSWORD IS TOO SHORT';
            END IF;
        END;
END
$$ LANGUAGE plpgsql;
   
