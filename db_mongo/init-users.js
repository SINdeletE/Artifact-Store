// Role initialization for ArtifactStore MongoDB
// Mirrors PostgreSQL GRANT structure: default_user, moderator, administrator

const db = db.getSiblingDB("artifactstore");

// ---------- default_user ----------
db.createRole({
  role: "default_user",
  privileges: [
    // characters: SELECT, INSERT, DELETE
    {
      resource: { db: "artifactstore", collection: "characters" },
      actions: ["find", "insert", "remove"],
    },
    // artifacts: SELECT
    {
      resource: { db: "artifactstore", collection: "artifacts" },
      actions: ["find"],
    },
    // currencies: SELECT
    {
      resource: { db: "artifactstore", collection: "currencies" },
      actions: ["find"],
    },
    // inventory_items: SELECT, DELETE
    {
      resource: { db: "artifactstore", collection: "inventory_items" },
      actions: ["find", "remove"],
    },
    // balances: SELECT, INSERT, DELETE
    {
      resource: { db: "artifactstore", collection: "balances" },
      actions: ["find", "insert", "remove"],
    },
    // store_items: SELECT
    {
      resource: { db: "artifactstore", collection: "store_items" },
      actions: ["find"],
    },
    // account_roles: SELECT
    {
      resource: { db: "artifactstore", collection: "account_roles" },
      actions: ["find"],
    },
    // accounts: SELECT
    {
      resource: { db: "artifactstore", collection: "accounts" },
      actions: ["find"],
    },
  ],
  roles: [],
});

// ---------- moderator ----------
db.createRole({
  role: "moderator",
  privileges: [
    // characters: SELECT, INSERT, DELETE, UPDATE
    {
      resource: { db: "artifactstore", collection: "characters" },
      actions: ["find", "insert", "remove", "update"],
    },
    // artifacts: SELECT, UPDATE
    {
      resource: { db: "artifactstore", collection: "artifacts" },
      actions: ["find", "update"],
    },
    // currencies: SELECT
    {
      resource: { db: "artifactstore", collection: "currencies" },
      actions: ["find"],
    },
    // inventory_items: SELECT, INSERT, DELETE
    {
      resource: { db: "artifactstore", collection: "inventory_items" },
      actions: ["find", "insert", "remove"],
    },
    // balances: SELECT, INSERT, DELETE, UPDATE
    {
      resource: { db: "artifactstore", collection: "balances" },
      actions: ["find", "insert", "remove", "update"],
    },
    // store_items: SELECT, INSERT, DELETE, UPDATE
    {
      resource: { db: "artifactstore", collection: "store_items" },
      actions: ["find", "insert", "remove", "update"],
    },
    // account_roles: SELECT
    {
      resource: { db: "artifactstore", collection: "account_roles" },
      actions: ["find"],
    },
    // accounts: SELECT, UPDATE
    {
      resource: { db: "artifactstore", collection: "accounts" },
      actions: ["find", "update"],
    },
    // store_transaction_types: SELECT
    {
      resource: { db: "artifactstore", collection: "store_transaction_types" },
      actions: ["find"],
    },
    // store_transaction_statuses: SELECT
    {
      resource: { db: "artifactstore", collection: "store_transaction_statuses" },
      actions: ["find"],
    },
    // store_transactions: SELECT
    {
      resource: { db: "artifactstore", collection: "store_transactions" },
      actions: ["find"],
    },
  ],
  roles: [],
});

// ---------- administrator ----------
// anyAction on all collections in the database
db.createRole({
  role: "administrator",
  privileges: [
    {
      resource: { db: "artifactstore", collection: "" },
      actions: ["anyAction"],
    },
  ],
  roles: [],
});

// ---------- Users ----------
db.createUser({
  user: process.env.MONGODB_DEFAULT_USER_NAME,
  pwd: process.env.MONGODB_DEFAULT_USER_PASSWORD,
  roles: [{ role: "default_user", db: "artifactstore" }],
});

db.createUser({
  user: process.env.MONGODB_MODERATOR_NAME,
  pwd: process.env.MONGODB_MODERATOR_PASSWORD,
  roles: [{ role: "moderator", db: "artifactstore" }],
});

db.createUser({
  user: process.env.MONGODB_ADMINISTRATOR_NAME,
  pwd: process.env.MONGODB_ADMINISTRATOR_PASSWORD,
  roles: [{ role: "administrator", db: "artifactstore" }],
});
