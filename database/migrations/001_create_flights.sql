-- ===============================================================
-- SISAND AIRLINES - ESTRUTURA BASE
-- ===============================================================

CREATE TABLE IF NOT EXISTS aircraft (
    id SERIAL PRIMARY KEY,
    model TEXT NOT NULL,
    code TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS flight_template (
    id SERIAL PRIMARY KEY,
    origin TEXT NOT NULL,
    destination TEXT NOT NULL,
    duration_minutes INT NOT NULL CHECK (duration_minutes > 0)
);

CREATE TABLE IF NOT EXISTS fare (
    id SERIAL PRIMARY KEY,
    class TEXT NOT NULL UNIQUE CHECK (class IN ('ECONOMY', 'FIRST')),
    amount NUMERIC(10,2) NOT NULL CHECK (amount > 0)
);

CREATE TABLE IF NOT EXISTS aircraft_seat (
    id SERIAL PRIMARY KEY,
    aircraft_id INT NOT NULL REFERENCES aircraft(id) ON DELETE CASCADE,
    seat_code TEXT NOT NULL,
    class TEXT NOT NULL CHECK (class IN ('ECONOMY', 'FIRST')),
    UNIQUE (aircraft_id, seat_code)
);

-- ===============================================================
-- SEED
-- ===============================================================
INSERT INTO aircraft (model, code)
VALUES 
('Airbus A370', 'A370-1'),
('Airbus A370', 'A370-2'),
('Airbus A370', 'A370-3')
ON CONFLICT (code) DO NOTHING;

INSERT INTO flight_template (origin, destination, duration_minutes)
VALUES ('Curitiba', 'São Paulo', 60)
ON CONFLICT DO NOTHING;

INSERT INTO fare (class, amount)
VALUES
('ECONOMY', 159.97),
('FIRST', 399.93)
ON CONFLICT (class) DO NOTHING;

DO $$
DECLARE
    a_id INT;
    s_code TEXT;
BEGIN
    FOR a_id IN SELECT id FROM aircraft LOOP
        
        FOR s_code IN SELECT unnest(ARRAY['1A','1B','1C','1D','1E']) LOOP
            INSERT INTO aircraft_seat (aircraft_id, seat_code, class)
            VALUES (a_id, s_code, 'ECONOMY')
            ON CONFLICT DO NOTHING;
        END LOOP;
        
        FOR s_code IN SELECT unnest(ARRAY['2A','2B']) LOOP
            INSERT INTO aircraft_seat (aircraft_id, seat_code, class)
            VALUES (a_id, s_code, 'FIRST')
            ON CONFLICT DO NOTHING;
        END LOOP;
    END LOOP;
END $$;
