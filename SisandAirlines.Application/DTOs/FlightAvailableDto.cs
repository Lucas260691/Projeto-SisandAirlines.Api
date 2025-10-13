namespace SisandAirlines.Application.DTOs
{
    public class FlightAvailableDto
    {
        public int Id { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string AircraftModel { get; set; } = string.Empty;
        public DateTime DepartureAt { get; set; }
        public DateTime ArrivalAt { get; set; }
        public string FareClass { get; set; } = string.Empty;
        public decimal BaseFare { get; set; }
    }
}
