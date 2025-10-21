namespace SisandAirlines.Application.DTOs
{
    public class FareAvailabilityDto
    {
        public string FareClass { get; set; } = string.Empty;
        public decimal BaseFare { get; set; }
        public int AvailableSeats { get; set; }
        public bool CanBook { get; set; }
    }
}
