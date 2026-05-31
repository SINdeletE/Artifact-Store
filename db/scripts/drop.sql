drop table account_roles cascade;
drop table accounts cascade;
drop table artifacts cascade;
drop table balances cascade;
drop table characters cascade;
drop table currencies cascade;
drop table inventory_items cascade;
drop table store_items cascade;
drop table store_transactions cascade;
drop table store_transaction_types cascade;
drop table store_transaction_statuses cascade;
drop table discounts cascade;

drop PROCEDURE buy_item(uuid, uuid);
drop PROCEDURE sell_item(uuid, uuid, numeric);
drop PROCEDURE register_account(account_id uuid, role_name varchar, nickname varchar, password varchar, email varchar, register_date date);

-- Roles
DROP ROLE default_user;
DROP ROLE moderator;
DROP ROLE administrator;
