import argparse
import getpass
import os
import sys

import jwt
import requests
from dotenv import load_dotenv

load_dotenv()

SECRET_KEY = os.getenv("SecretKey")
AUTH_API_URL = f"http://localhost:{os.getenv('AUTH_API_PORT')}"
ADMIN_NICKNAME = os.getenv("CLI_NICKNAME")
ADMIN_PASSWORD = os.getenv("CLI_PASSWORD")


def print_accounts_table(items: list):
    headers = ["#", "ID", "Role", "Nickname", "Email", "Register Date"]
    rows = [
        [
            str(i + 1),
            v["id"],
            v["accountRole"]["name"],
            v["nickname"],
            v["email"],
            v["registerDate"],
        ]
        for i, v in enumerate(items)
    ]

    col_widths = [max(len(str(r[c])) for r in [headers] + rows) for c in range(len(headers))]
    sep = "+" + "+".join("-" * (w + 2) for w in col_widths) + "+"
    fmt = "|" + "|".join(f" {{:<{w}}} " for w in col_widths) + "|"

    print(sep)
    print(fmt.format(*headers))
    print(sep)
    for row in rows:
        print(fmt.format(*row))
    print(sep)


def get_token(nickname: str, password: str) -> str:
    response = requests.post(
        f"{AUTH_API_URL}/api/auth/login",
        json={"nickname": nickname, "password": password},
    )
    if response.status_code != 200:
        print(f"Login failed with code {response.status_code}")
        sys.exit(1)

    login_token = response.json()["token"]
    if not is_admin(login_token, SECRET_KEY):
        print("User is not an admin")
        sys.exit(1)

    return login_token


def is_admin(token: str, secret_key: str) -> bool:
    try:
        payload = jwt.decode(
            token,
            secret_key,
            algorithms=["HS256"],
            options={"verify_aud": False},
        )
        return payload.get("role") == "admin"
    except (jwt.ExpiredSignatureError, jwt.InvalidTokenError):
        return False


def cmd_view_accounts(args, token: str):
    response = requests.get(
        f"{AUTH_API_URL}/api/account",
        params={"page": args.page, "pageSize": args.page_size},
        headers={"Authorization": f"Bearer {token}"},
    )
    if response.status_code == 200:
        print_accounts_table(response.json()["items"])
    else:
        print(f"View failed with code {response.status_code}: {response.text}")
        sys.exit(1)


def cmd_create_account(args, token: str):
    response = requests.post(
        f"{AUTH_API_URL}/api/account",
        json={
            "role": args.role,
            "nickname": args.nickname,
            "password": args.user_password,
            "email": args.email,
        },
        headers={"Authorization": f"Bearer {token}"},
    )
    if response.status_code == 200:
        print("successfully added")
    else:
        print(f"Create failed with code {response.status_code}: {response.text}")
        sys.exit(1)


def cmd_delete_account(args, token: str):
    response = requests.delete(
        f"{AUTH_API_URL}/api/account/{args.account_id}",
        headers={"Authorization": f"Bearer {token}"},
    )
    if response.status_code == 200:
        print("successfully deleted")
    elif response.status_code == 404:
        print("account not found")
        sys.exit(1)
    else:
        print(f"Delete failed with code {response.status_code}: {response.text}")
        sys.exit(1)


def main():
    parser = argparse.ArgumentParser(prog="admin-cli")

    subparsers = parser.add_subparsers(dest="command", required=True)

    view_parser = subparsers.add_parser("view-accounts", help="List accounts (paginated)")
    view_parser.add_argument("--page", type=int, required=True)
    view_parser.add_argument("--page-size", type=int, required=True, dest="page_size")

    create_parser = subparsers.add_parser("create-account", help="Create a new account")
    create_parser.add_argument("--nickname", required=True)
    create_parser.add_argument("--user-password", required=True, dest="user_password")
    create_parser.add_argument("--email", required=True)
    create_parser.add_argument("--role", required=True)
    
    delete_parser = subparsers.add_parser("delete-account", help="Delete an account by ID")
    delete_parser.add_argument("--id", required=True, dest="account_id")

    args = parser.parse_args()

    admin_login = ADMIN_NICKNAME or input("nickname: ")
    admin_password = ADMIN_PASSWORD or getpass.getpass("password: ")

    token = get_token(admin_login, admin_password)

    match args.command:
        case "view-accounts":
            cmd_view_accounts(args, token)
        case "create-account":
            cmd_create_account(args, token)
        case "delete-account":
            cmd_delete_account(args, token)


if __name__ == "__main__":
    main()
