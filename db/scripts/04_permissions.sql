-- Roles
CREATE ROLE default_user WITH LOGIN PASSWORD 'qweqv_wxVXGV__wuqveqie145';
CREATE ROLE moderator WITH LOGIN PASSWORD 'efbiwcbB_____enwqo_cn24';
CREATE ROLE administrator WITH LOGIN PASSWORD 'jfvnjvnhbrbeu_2123_12jep1j_1';

-- connection
GRANT CONNECT ON DATABASE "ArtifactStore" TO default_user;
GRANT CONNECT ON DATABASE "ArtifactStore" TO moderator;
GRANT CONNECT ON DATABASE "ArtifactStore" TO administrator;

-- schema
GRANT USAGE ON SCHEMA public TO default_user;
GRANT USAGE ON SCHEMA public TO moderator;
GRANT USAGE ON SCHEMA public TO administrator;

-- procedures
GRANT EXECUTE ON PROCEDURE buy_item  TO default_user;
GRANT EXECUTE ON PROCEDURE sell_item TO default_user;
GRANT EXECUTE ON PROCEDURE register_account TO default_user;
GRANT EXECUTE ON PROCEDURE register_account TO moderator;

-- tables
GRANT SELECT, INSERT, DELETE         ON characters TO default_user;
GRANT SELECT, INSERT, DELETE, UPDATE ON characters TO moderator;

GRANT SELECT         ON artifacts TO default_user;
GRANT SELECT, UPDATE ON artifacts TO moderator;

GRANT SELECT ON currencies TO default_user;
GRANT SELECT ON currencies TO moderator;

GRANT SELECT,         DELETE ON inventory_items TO default_user;
GRANT SELECT, INSERT, DELETE ON inventory_items TO moderator;

GRANT SELECT, INSERT, DELETE         ON balances TO default_user;
GRANT SELECT, INSERT, DELETE, UPDATE ON balances TO moderator;

GRANT SELECT                         ON store_items TO default_user;
GRANT SELECT, INSERT, DELETE, UPDATE ON store_items TO moderator;

GRANT SELECT         ON account_roles TO default_user;
GRANT SELECT         ON account_roles TO moderator;
GRANT SELECT         ON accounts TO default_user;
GRANT SELECT, UPDATE ON accounts TO moderator;

GRANT SELECT ON store_transaction_types TO moderator;
GRANT SELECT ON store_transaction_statuses TO moderator;
GRANT SELECT ON store_transactions TO moderator;

-- Admin
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO administrator;
