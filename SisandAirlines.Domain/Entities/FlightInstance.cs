namespace SisandAirlines.Domain.Entities
{
    public class FlightInstance
    {
        public int Id { get; set; }

        // 🔹 Chaves estrangeiras
        public int FlightTemplateId { get; set; }
        public int AircraftId { get; set; }

        // 🔹 Informações da instância real do voo
        public DateTime DepartureAt { get; set; }
        public DateTime ArrivalAt { get; set; }

        // 🔹 Informações desnormalizadas (preenchidas pelo JOIN no repositório)
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string AircraftModel { get; set; } = string.Empty;
        public string AircraftCode { get; set; } = string.Empty;
        public string FareClass { get; set; } = string.Empty;
        public decimal BaseFare { get; set; }
    }
}
