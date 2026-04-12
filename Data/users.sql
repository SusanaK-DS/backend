-- App-owned table (avoids clashing with an existing public.users table that may have a different shape).

CREATE TABLE IF NOT EXISTS library_users (
    id SERIAL PRIMARY KEY,
    name CHARACTER VARYING(200) NOT NULL,
    email CHARACTER VARYING(320) NOT NULL UNIQUE
);

-- Optional: copy from a legacy table if columns match (adjust source table/columns as needed):
-- INSERT INTO library_users (name, email)
-- SELECT name, email FROM users
-- ON CONFLICT DO NOTHING;
