using SisandAirlines.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisandAirlines.Domain.Interfaces
{
    public interface IFlightRepository : IRepository<FlightInstance>
    {
        Task<IEnumerable<dynamic>> GetAvailableFlightsAsync(DateTime date);
    }
}
