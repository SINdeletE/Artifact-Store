INSERT INTO account_roles
values (gen_random_uuid(), 'admin');
INSERT INTO account_roles
values (gen_random_uuid(), 'moderator');
INSERT INTO account_roles
values (gen_random_uuid(), 'default_user');

INSERT INTO store_transaction_types
values (gen_random_uuid(), 'purchase');
INSERT INTO store_transaction_types
values (gen_random_uuid(), 'sell');

INSERT INTO store_transaction_statuses
values (gen_random_uuid(), 'success');
INSERT INTO store_transaction_statuses
values (gen_random_uuid(), 'denied');

INSERT INTO accounts
values (gen_random_uuid(), (select id from account_roles where name = 'admin'), 'alexo', '$2a$12$1Qhgoafc3tw6uHubxZjPuugOVPtpVrEjTOZLlW8EVpifp3xm1Q8g6', 'meme@mail.ru', now(), null);


INSERT INTO characters
values (gen_random_uuid(), (select id from accounts where nickname = 'alexo' LIMIT 1), 'qwerty');
INSERT INTO artifacts
values (gen_random_uuid(), 'megasword');
INSERT INTO currencies
values (gen_random_uuid(), 'gold');
INSERT INTO balances
values (gen_random_uuid(), (select id from characters where name = 'qwerty' AND characters.deleted_at is null LIMIT 1), (select id from currencies where name = 'gold' LIMIT 1), 10000);
INSERT INTO store_items
values (gen_random_uuid(), (select id from currencies where name = 'gold' LIMIT 1), (select id from artifacts where name = 'megasword' LIMIT 1), 500);
