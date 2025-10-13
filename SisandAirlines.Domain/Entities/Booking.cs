namespace SisandAirlines.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int FlightInstanceId { get; set; }
        public int SeatId { get; set; }
        public int FareId { get; set; }
        public DateTime BookingDate { get; set; }
        public string PaymentStatus { get; set; } = "PENDING"; // PENDING | PAID | CANCELLED
    }
}
