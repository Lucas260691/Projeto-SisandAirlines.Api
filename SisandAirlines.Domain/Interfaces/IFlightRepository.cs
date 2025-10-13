using SisandAirlines.Domain.Entities;

namespace SisandAirlines.Domain.Interfaces
{
    public interface IFlightRepository : IRepository<FlightInstance>
    {
        Task<IEnumerable<FlightInstance>> GetAvailableFlightsAsync(DateTime date);
    }
}
