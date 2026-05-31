from __future__ import annotations

import csv
import os
import random
import uuid
from datetime import timezone
from decimal import Decimal
from pathlib import Path

try:
    import bcrypt
    from faker import Faker
    import psycopg2
    from psycopg2.extras import execute_values
except ImportError as exc:
    raise SystemExit(
        "Install dependencies before running: pip install bcrypt faker psycopg2-binary"
    ) from exc


CONNECTION_STRING_ENV_KEY = "ConnectionStrings__Owner"
ACCOUNTS_OUTPUT_FILE = Path(__file__).with_name("generated_accounts.csv")

ACCOUNTS_COUNT = 100
DEFAULT_ROWS_COUNT = 1000
CURRENCIES_COUNT = 5
PAGE_SIZE = 500

ACCOUNT_ROLES = ("admin", "moderator", "default_user")
STORE_TRANSACTION_TYPES = ("purchase", "sell")
STORE_TRANSACTION_STATUSES = ("success", "denied")
CURRENCIES = (
    ("gold", Decimal("1.00")),
    ("gems", Decimal("3.50")),
    ("crystals", Decimal("7.25")),
    ("honor_points", Decimal("0.80")),
    ("ancient_coins", Decimal("12.00")),
)

fake = Faker("en_US")
Faker.seed(42)
random.seed(42)


def find_env_file() -> Path | None:
    scripts_dir = Path(__file__).resolve().parent
    for directory in (scripts_dir, *scripts_dir.parents):
        env_file = directory / ".env"
        if env_file.exists():
            return env_file
    return None


def parse_env_value(value: str) -> str:
    value = value.strip()
    if len(value) >= 2 and value[0] == value[-1] and value[0] in ("'", '"'):
        return value[1:-1]
    return value


def load_connection_string() -> str:
    connection_string = os.getenv(CONNECTION_STRING_ENV_KEY)
    if connection_string:
        return connection_string

    env_file = find_env_file()
    if env_file:
        with env_file.open(encoding="utf-8") as file:
            for raw_line in file:
                line = raw_line.strip()
                if not line or line.startswith("#") or "=" not in line:
                    continue

                key, value = line.split("=", 1)
                if key.strip() == CONNECTION_STRING_ENV_KEY:
                    return parse_env_value(value)

    raise SystemExit(
        f"Set {CONNECTION_STRING_ENV_KEY} in .env or environment variables"
    )


def new_id() -> str:
    return str(uuid.uuid4())


def money(min_value: Decimal, max_value: Decimal) -> Decimal:
    value = random.uniform(float(min_value), float(max_value))
    return Decimal(str(value)).quantize(Decimal("0.0001"))


def hash_password(password: str) -> str:
    return bcrypt.hashpw(
        password.encode("utf-8"),
        bcrypt.gensalt(rounds=12),
    ).decode("utf-8")


def insert_rows(cursor, table: str, columns: tuple[str, ...], rows: list[tuple]) -> None:
    if not rows:
        return

    column_sql = ", ".join(columns)
    execute_values(
        cursor,
        f"INSERT INTO {table} ({column_sql}) VALUES %s",
        rows,
        page_size=PAGE_SIZE,
    )


def seed_named_rows(cursor, table: str, names: tuple[str, ...]) -> list[dict[str, str]]:
    rows = [(new_id(), name) for name in names]
    execute_values(
        cursor,
        f"INSERT INTO {table} (id, name) VALUES %s ON CONFLICT (name) DO NOTHING",
        rows,
    )
    cursor.execute(
        f"SELECT id::text, name FROM {table} WHERE name = ANY(%s) ORDER BY name",
        (list(names),),
    )
    return [{"id": row[0], "name": row[1]} for row in cursor.fetchall()]


def seed_currencies(cursor) -> list[dict[str, str | Decimal]]:
    rows = [(new_id(), name) for name, _ in CURRENCIES[:CURRENCIES_COUNT]]
    execute_values(
        cursor,
        "INSERT INTO currencies (id, name) VALUES %s ON CONFLICT (name) DO NOTHING",
        rows,
    )
    currency_names = [name for name, _ in CURRENCIES[:CURRENCIES_COUNT]]
    multipliers = dict(CURRENCIES[:CURRENCIES_COUNT])
    cursor.execute(
        "SELECT id::text, name FROM currencies WHERE name = ANY(%s) ORDER BY name",
        (currency_names,),
    )
    return [
        {"id": row[0], "name": row[1], "multiplier": multipliers[row[1]]}
        for row in cursor.fetchall()
    ]


def generate_accounts(roles: list[dict[str, str]], run_tag: str) -> list[dict[str, str]]:
    default_role = next(role for role in roles if role["name"] == "default_user")
    moderator_role = next(role for role in roles if role["name"] == "moderator")
    role_pool = [default_role] * 88 + [moderator_role] * 12

    accounts = []
    for index in range(ACCOUNTS_COUNT):
        nickname = f"{fake.user_name()[:18]}_{run_tag}_{index:04d}"
        password = fake.password(
            length=16,
            special_chars=True,
            digits=True,
            upper_case=True,
            lower_case=True,
        )
        role = random.choice(role_pool)
        accounts.append(
            {
                "id": new_id(),
                "role_id": role["id"],
                "role_name": role["name"],
                "nickname": nickname,
                "password": password,
                "password_hash": hash_password(password),
                "email": f"{nickname}@generated.local",
                "register_date": fake.date_between(start_date="-4y", end_date="today"),
            }
        )
    return accounts


def write_accounts_file(accounts: list[dict[str, str]]) -> None:
    with ACCOUNTS_OUTPUT_FILE.open("w", newline="", encoding="utf-8") as file:
        writer = csv.DictWriter(
            file,
            fieldnames=(
                "id",
                "role_name",
                "nickname",
                "password",
                "email",
                "register_date",
            ),
        )
        writer.writeheader()
        for account in accounts:
            writer.writerow(
                {
                    "id": account["id"],
                    "role_name": account["role_name"],
                    "nickname": account["nickname"],
                    "password": account["password"],
                    "email": account["email"],
                    "register_date": account["register_date"],
                }
            )


def generate_characters(accounts: list[dict[str, str]], run_tag: str) -> list[dict[str, str]]:
    return [
        {
            "id": new_id(),
            "account_id": random.choice(accounts)["id"],
            "name": f"char_{run_tag}_{index:04d}",
            "creation_date": fake.date_between(start_date="-3y", end_date="today"),
        }
        for index in range(DEFAULT_ROWS_COUNT)
    ]


def generate_artifacts(run_tag: str) -> list[dict[str, str]]:
    rarities = ("common", "rare", "epic", "legendary", "ancient")
    item_kinds = ("sword", "amulet", "ring", "shield", "staff", "cloak", "bow")
    return [
        {
            "id": new_id(),
            "name": f"{random.choice(rarities)}_{random.choice(item_kinds)}_{run_tag}_{index:04d}",
            "description": fake.sentence(nb_words=14)[:512],
        }
        for index in range(DEFAULT_ROWS_COUNT)
    ]


def generate_inventory_items(
    characters: list[dict[str, str]],
    artifacts: list[dict[str, str]],
) -> list[tuple]:
    return [
        (
            new_id(),
            random.choice(characters)["id"],
            random.choice(artifacts)["id"],
        )
        for _ in range(DEFAULT_ROWS_COUNT)
    ]


def generate_balances(
    characters: list[dict[str, str]],
    currencies: list[dict[str, str | Decimal]],
) -> list[dict[str, str | Decimal]]:
    return [
        {
            "id": new_id(),
            "character_id": random.choice(characters)["id"],
            "currency_id": random.choice(currencies)["id"],
            "amount": money(Decimal("0"), Decimal("25000")),
        }
        for _ in range(DEFAULT_ROWS_COUNT)
    ]


def generate_store_items(
    artifacts: list[dict[str, str]],
    currencies: list[dict[str, str | Decimal]],
) -> list[tuple]:
    rows = []
    for _ in range(DEFAULT_ROWS_COUNT):
        currency = random.choice(currencies)
        base_price = money(Decimal("5"), Decimal("600"))
        price = (base_price * currency["multiplier"]).quantize(Decimal("0.0001"))
        rows.append(
            (
                new_id(),
                currency["id"],
                random.choice(artifacts)["id"],
                price,
            )
        )
    return rows


def generate_store_transactions(
    balances: list[dict[str, str | Decimal]],
    artifacts: list[dict[str, str]],
    transaction_types: list[dict[str, str]],
    statuses: list[dict[str, str]],
) -> list[tuple]:
    return [
        (
            new_id(),
            random.choice(balances)["id"],
            random.choice(artifacts)["id"],
            random.choice(transaction_types)["id"],
            random.choice(statuses)["id"],
            money(Decimal("1"), Decimal("5000")),
            fake.date_time_between(
                start_date="-2y",
                end_date="now",
                tzinfo=timezone.utc,
            ),
        )
        for _ in range(DEFAULT_ROWS_COUNT)
    ]


def main() -> None:
    run_tag = uuid.uuid4().hex[:6]
    accounts = []

    with psycopg2.connect(load_connection_string()) as connection:
        with connection.cursor() as cursor:
            roles = seed_named_rows(cursor, "account_roles", ACCOUNT_ROLES)
            transaction_types = seed_named_rows(
                cursor,
                "store_transaction_types",
                STORE_TRANSACTION_TYPES,
            )
            statuses = seed_named_rows(
                cursor,
                "store_transaction_statuses",
                STORE_TRANSACTION_STATUSES,
            )
            currencies = seed_currencies(cursor)

            accounts = generate_accounts(roles, run_tag)
            insert_rows(
                cursor,
                "accounts",
                (
                    "id",
                    "role_id",
                    "nickname",
                    "password",
                    "email",
                    "register_date",
                ),
                [
                    (
                        account["id"],
                        account["role_id"],
                        account["nickname"],
                        account["password_hash"],
                        account["email"],
                        account["register_date"],
                    )
                    for account in accounts
                ],
            )

            characters = generate_characters(accounts, run_tag)
            insert_rows(
                cursor,
                "characters",
                ("id", "account_id", "name", "creation_date"),
                [
                    (
                        character["id"],
                        character["account_id"],
                        character["name"],
                        character["creation_date"],
                    )
                    for character in characters
                ],
            )

            artifacts = generate_artifacts(run_tag)
            insert_rows(
                cursor,
                "artifacts",
                ("id", "name", "description"),
                [
                    (
                        artifact["id"],
                        artifact["name"],
                        artifact["description"],
                    )
                    for artifact in artifacts
                ],
            )

            insert_rows(
                cursor,
                "inventory_items",
                ("id", "character_id", "artifact_id"),
                generate_inventory_items(characters, artifacts),
            )

            balances = generate_balances(characters, currencies)
            insert_rows(
                cursor,
                "balances",
                ("id", "character_id", "currency_id", "amount"),
                [
                    (
                        balance["id"],
                        balance["character_id"],
                        balance["currency_id"],
                        balance["amount"],
                    )
                    for balance in balances
                ],
            )

            insert_rows(
                cursor,
                "store_items",
                ("id", "currency_id", "artifact_id", "price"),
                generate_store_items(artifacts, currencies),
            )

            insert_rows(
                cursor,
                "store_transactions",
                (
                    "id",
                    "balance_id",
                    "artifact_id",
                    "type_id",
                    "status_id",
                    "amount",
                    "transaction_datetime",
                ),
                generate_store_transactions(
                    balances,
                    artifacts,
                    transaction_types,
                    statuses,
                ),
            )

    write_accounts_file(accounts)
    print(f"Generated account credentials: {ACCOUNTS_OUTPUT_FILE}")


if __name__ == "__main__":
    main()
