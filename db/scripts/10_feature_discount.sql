CREATE TABLE discounts (
    id uuid primary key default gen_random_uuid(),
    name varchar(64) not null,
    description varchar(512) not null default '',
    percent decimal not null,
    starts_at timestamp with time zone not null default now(),
    ends_at timestamp with time zone not null default now(),
    deleted_at timestamp with time zone default null
);
ALTER TABLE discounts ADD CONSTRAINT discount_percent_limit CHECK ( percent >= 0 AND percent <= 1 );
ALTER TABLE discounts ADD CONSTRAINT discount_name_noempty CHECK (name <> '');

ALTER TABLE store_items ADD COLUMN discount_id uuid;
ALTER TABLE store_items ADD CONSTRAINT fk_store_items_discounts FOREIGN KEY (discount_id) REFERENCES discounts(id) ON DELETE RESTRICT;

-- PERMISSIONS
GRANT SELECT                         ON discounts TO default_user;
GRANT SELECT, INSERT, DELETE, UPDATE ON discounts TO moderator;
GRANT ALL PRIVILEGES                 ON discounts TO administrator;

-- buy_item with discounts influence
CREATE OR REPLACE PROCEDURE buy_item (
        p_store_item_id uuid,
        p_character_id uuid
)
SECURITY DEFINER
AS $$
DECLARE
client_b balances%ROWTYPE;
        client_s_i store_items%ROWTYPE;
        total_price numeric(19, 4);
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
        
        total_price := client_s_i.price * (1 - COALESCE((select percent from discounts d where d.id = client_s_i.discount_id LIMIT 1), 0));
        
        IF client_b.amount < total_price THEN
            RAISE EXCEPTION 'BALANCE NOT ENOUGH';
        END IF;
        
                -- DELETE FROM STORE ITEMS
        DELETE FROM store_items WHERE id = p_store_item_id;
        GET DIAGNOSTICS row_count = ROW_COUNT;
        IF row_count = 0 THEN
            RAISE EXCEPTION 'CONFLICT';
        END IF;
                
                -- UPDATE BALANCE
        UPDATE balances SET amount = amount - total_price WHERE id = client_b.id;
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
   
-- TEST DATA
INSERT INTO discounts
values (gen_random_uuid(), 'secret friday', 'Secret Friday came to Artifact Store!', 0.25, now(), now() + interval '2 days');
INSERT INTO artifacts
values (gen_random_uuid(), 'megasword_for_sale');
INSERT INTO store_items
values (gen_random_uuid(), (select id from currencies where name = 'gold' LIMIT 1), (select id from artifacts where name = 'megasword_for_sale' LIMIT 1), 500, (select id from discounts where name = 'secret friday' LIMIT 1));
   