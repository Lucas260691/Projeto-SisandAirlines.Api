namespace SisandAirlines.Application.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int FlightInstanceId { get; set; }
        public int SeatId { get; set; }
        public int FareId { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
    }

    public class CreateBookingRequest
    {
        public int CustomerId { get; set; }
        public int FlightInstanceId { get; set; }
        public int SeatId { get; set; }
        public int FareId { get; set; }
    }
}
