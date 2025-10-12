using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisandAirlines.Domain.Entities
{
    public class FlightTemplate
    {
        public int Id { get; set; }
        public int AircraftId { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public decimal BaseFare { get; set; }
    }
}
