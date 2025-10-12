-- �ndices de performance
CREATE INDEX IF NOT EXISTS idx_booking_customer ON booking (customer_id);
CREATE INDEX IF NOT EXISTS idx_booking_flight ON booking (flight_instance_id);
CREATE INDEX IF NOT EXISTS idx_booking_status ON booking (payment_status);
