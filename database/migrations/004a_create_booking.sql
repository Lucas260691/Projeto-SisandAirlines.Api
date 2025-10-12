-- ===============================================================
-- SISAND AIRLINES - RESERVAS DE PASSAGENS
-- ===============================================================

CREATE TABLE IF NOT EXISTS booking (
    id SERIAL PRIMARY KEY,
    customer_id INT NOT NULL REFERENCES customer(id) ON DELETE CASCADE,
    flight_instance_id INT NOT NULL REFERENCES flight_instance(id) ON DELETE CASCADE,
    seat_id INT NOT NULL REFERENCES aircraft_seat(id) ON DELETE CASCADE,
    fare_id INT NOT NULL REFERENCES fare(id),
    booking_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    payment_status TEXT NOT NULL CHECK (payment_status IN ('PENDING', 'PAID', 'CANCELLED')),
    UNIQUE (flight_instance_id, seat_id), 
    UNIQUE (customer_id, flight_instance_id) 
);

