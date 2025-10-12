-- ===============================================================
-- SISAND AIRLINES - PAGAMENTOS E AUDITORIA
-- ===============================================================

CREATE TABLE IF NOT EXISTS payment (
    id SERIAL PRIMARY KEY,
    booking_id INT NOT NULL REFERENCES booking(id) ON DELETE CASCADE,
    method TEXT NOT NULL CHECK (method IN ('PIX')),
    amount NUMERIC(10,2) NOT NULL CHECK (amount > 0),
    paid_at TIMESTAMP,
    confirmation_code TEXT UNIQUE
);

CREATE TABLE IF NOT EXISTS audit_log (
    id SERIAL PRIMARY KEY,
    table_name TEXT NOT NULL,
    operation TEXT NOT NULL CHECK (operation IN ('INSERT', 'UPDATE', 'DELETE')),
    record_id INT NOT NULL,
    user_email TEXT,
    log_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Trigger gen�rica para auditoria
CREATE OR REPLACE FUNCTION log_audit() RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO audit_log (table_name, operation, record_id, user_email)
    VALUES (TG_TABLE_NAME, TG_OP, NEW.id, current_setting('app.user', true));
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_booking_audit
AFTER INSERT OR UPDATE OR DELETE ON booking
FOR EACH ROW EXECUTE FUNCTION log_audit();
