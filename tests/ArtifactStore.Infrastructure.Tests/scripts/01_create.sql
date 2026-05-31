CREATE TABLE account_roles (
    id uuid primary key default gen_random_uuid(),
    name varchar(32) not null unique
);
CREATE TABLE accounts (
    id uuid primary key default gen_random_uuid(),
    role_id uuid not null references account_roles(id) on delete restrict,
    nickname varchar(32) not null unique,
    password varchar(256) not null,
    email varchar(254) not null unique,
    register_date date not null default now(),
    deleted_at timestamp with time zone default null
);

CREATE TABLE characters (
    id uuid primary key default gen_random_uuid(),
    account_id uuid not null references accounts(id) on delete restrict,
    name varchar(32) not null,
    creation_date date not null default now(),
    deleted_at timestamp with time zone default null
);

CREATE TABLE artifacts (
    id uuid primary key default gen_random_uuid(),
    name varchar(128) not null unique,
    description varchar(512) not null default '',
    deleted_at timestamp with time zone default null
);

CREATE TABLE currencies (
    id uuid primary key default gen_random_uuid(),
    name varchar(128) not null unique,
    deleted_at timestamp with time zone default null
);

CREATE TABLE inventory_items (
    id uuid primary key default gen_random_uuid(),
    character_id uuid not null references characters(id) on delete restrict,
    artifact_id uuid not null references artifacts(id) on delete restrict
);

CREATE TABLE balances (
    id uuid primary key default gen_random_uuid(),
    character_id uuid not null references characters(id) on delete restrict,
    currency_id uuid not null references currencies(id) on delete restrict,
    amount numeric(19, 4) not null default 0,
    deleted_at timestamp with time zone default null
);

CREATE TABLE store_items (
    id uuid primary key default gen_random_uuid(),
    currency_id uuid not null references currencies(id) on delete restrict,
    artifact_id uuid not null references artifacts(id) on delete restrict,
    price numeric(19, 4) not null default 0
);

CREATE TABLE store_transaction_types (
    id uuid primary key default gen_random_uuid(),
    name varchar(32) not null unique
);
CREATE TABLE store_transaction_statuses (
    id uuid primary key default gen_random_uuid(),
    name varchar(32) not null unique
);
CREATE TABLE store_transactions (
    id uuid primary key default gen_random_uuid(),
    balance_id uuid not null references balances(id) on delete restrict,
    artifact_id uuid not null references artifacts(id) on delete restrict,
    type_id uuid not null references store_transaction_types(id) on delete restrict,
    status_id uuid not null references store_transaction_statuses(id) on delete restrict,
    amount numeric(19, 4) not null default 0,
    transaction_datetime timestamp with time zone not null default now()
);
