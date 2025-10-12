-- Instâncias reais de voos (voos diários derivados do template)
CREATE TABLE IF NOT EXISTS flight_instance (
    id SERIAL PRIMARY KEY,
    flight_template_id INT NOT NULL REFERENCES flight_template(id) ON DELETE CASCADE,
    aircraft_id INT NOT NULL REFERENCES aircraft(id) ON DELETE RESTRICT,
    departure_at TIMESTAMP NOT NULL,
    arrival_at   TIMESTAMP NOT NULL,
    UNIQUE (flight_template_id, aircraft_id, departure_at)
);

DO $$
DECLARE
    t_id INT;
    a_id INT;
    d_hour INT;
    base_date DATE := CURRENT_DATE;
    dep_time TIMESTAMP;
    arr_time TIMESTAMP;
    aircrafts INT[];
    idx INT := 1;
BEGIN
    -- pega um template (Curitiba -> São Paulo, 60 min)
    SELECT id INTO t_id FROM flight_template LIMIT 1;

    -- lista de aeronaves disponíveis
    SELECT array_agg(id ORDER BY id) INTO aircrafts FROM aircraft;

    -- gera saídas de 3 em 3 horas no dia base
    FOR d_hour IN 0..21 BY 3 LOOP
        dep_time := base_date + make_interval(hours => d_hour);
        arr_time := dep_time + make_interval(hours => 1);
        a_id := aircrafts[idx];

        INSERT INTO flight_instance (flight_template_id, aircraft_id, departure_at, arrival_at)
        VALUES (t_id, a_id, dep_time, arr_time)
        ON CONFLICT DO NOTHING;

        idx := idx + 1;
        IF idx > array_length(aircrafts, 1) THEN
            idx := 1;
        END IF;
    END LOOP;
END $$;
