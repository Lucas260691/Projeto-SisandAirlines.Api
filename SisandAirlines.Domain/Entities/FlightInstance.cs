using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisandAirlines.Domain.Entities
{
    public class FlightInstance
    {
        public int Id { get; set; }
        public int FlightTemplateId { get; set; }
        public int AircraftId { get; set; }
        public DateTime DepartureAt { get; set; }
        public DateTime ArrivalAt { get; set; }
    }
}
