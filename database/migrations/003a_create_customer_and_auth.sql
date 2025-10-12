-- ===============================================================
-- SISAND AIRLINES - CLIENTES E LOGIN
-- ===============================================================

CREATE TABLE IF NOT EXISTS customer (
    id SERIAL PRIMARY KEY,
    full_name TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    cpf CHAR(11) NOT NULL UNIQUE,
    birth_date DATE NOT NULL,
    password_hash TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

