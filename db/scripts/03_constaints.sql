ALTER TABLE accounts ADD CONSTRAINT account_min_len_password CHECK (CHAR_LENGTH(password) >= 8);                                                                                                                                                  
CREATE UNIQUE INDEX accounts_nickname_unique ON accounts (nickname) WHERE deleted_at IS NULL;
CREATE UNIQUE INDEX accounts_email_unique ON accounts (email) WHERE deleted_at IS NULL;

ALTER TABLE characters ADD CONSTRAINT character_noempty CHECK (name <> '');

ALTER TABLE store_items ADD CONSTRAINT price_positive CHECK (price >= 0);


