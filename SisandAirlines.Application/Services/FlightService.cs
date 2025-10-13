using SisandAirlines.Application.DTOs;
using SisandAirlines.Domain.Interfaces;

namespace SisandAirlines.Application.Services
{
    public class FlightService
    {
        private readonly IUnitOfWork _uow;

        public FlightService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<FlightAvailableDto>> GetAvailableFlightsAsync(DateTime date)
        {
            var flights = await _uow.Flights.GetAvailableFlightsAsync(date);

            // converte entidades -> DTOs
            return flights.Select(f => new FlightAvailableDto
            {
                Id = f.Id,
                Origin = f.Origin,
                Destination = f.Destination,
                AircraftModel = f.AircraftModel,
                DepartureAt = f.DepartureAt,
                ArrivalAt = f.ArrivalAt,
                BaseFare = f.BaseFare,
                FareClass = f.FareClass
            });
        }
    }
}
