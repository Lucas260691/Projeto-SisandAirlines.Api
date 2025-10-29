using SisandAirlines.Application.DTOs;
using SisandAirlines.Domain.Entities;
using SisandAirlines.Domain.Interfaces;
using Dapper;

namespace SisandAirlines.Application.Services
{
    public class FlightService : IFlightScheduler
    {
        private readonly IUnitOfWork _uow;

        public FlightService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ================================================================
        // 1️⃣ Consulta voos disponíveis (com classes e assentos)
        // ================================================================
        public async Task<IEnumerable<FlightAvailableDto>> GetAvailableFlightsAsync(DateTime date, int? passengers = null)
        {
            if (date.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Não é possível consultar voos para datas passadas.");

            // Busca os voos disponíveis na data
            var flights = await _uow.Flights.GetAvailableFlightsAsync(date);

            if (!flights.Any())
                return Enumerable.Empty<FlightAvailableDto>();

            // Busca a configuração de assentos das aeronaves
            var seatData = await _uow.Connection.QueryAsync<(int AircraftId, string Class)>(
                "SELECT aircraft_id, class FROM aircraft_seat;",
                transaction: _uow.Transaction
            );

            // Agrupa por aeronave + classe para contar quantos assentos há
            var seatAvailability = seatData
                .GroupBy(s => new { s.AircraftId, s.Class })
                .ToDictionary(
                    g => (g.Key.AircraftId, g.Key.Class),
                    g => g.Count()
                );

            // Mapeia cada voo para incluir suas classes e disponibilidade
            return flights.Select(f =>
            {
                var economySeats = seatAvailability.TryGetValue((f.AircraftId, "ECONOMY"), out var econSeats) ? econSeats : 0;
                var firstSeats = seatAvailability.TryGetValue((f.AircraftId, "FIRST"), out var firstClassSeats) ? firstClassSeats : 0;

                return new FlightAvailableDto
                {
                    Id = f.Id,
                    Origin = f.Origin,
                    Destination = f.Destination,
                    AircraftModel = f.AircraftModel,
                    DepartureAt = f.DepartureAt,
                    ArrivalAt = f.ArrivalAt,
                    Classes = new List<FareAvailabilityDto>
                    {
                        new()
                        {
                            FareClass = "ECONOMY",
                            BaseFare = 159.97m,
                            AvailableSeats = economySeats,
                            CanBook = passengers == null || passengers <= economySeats
                        },
                        new()
                        {
                            FareClass = "FIRST_CLASS",
                            BaseFare = 399.93m,
                            AvailableSeats = firstSeats,
                            CanBook = passengers == null || passengers <= firstSeats
                        }
                    }
                };
            });
        }

        // ================================================================
        // 2️⃣ Geração contínua de voos (janela de 60 dias)
        // ================================================================
        public async Task GenerateFutureFlightsAsync(int daysAhead = 60)
        {
            var flightRepo = _uow.Flights;

            var templates = (await _uow.Connection.QueryAsync<dynamic>(
                "SELECT id FROM flight_template;", transaction: _uow.Transaction)).ToList();

            var aircrafts = (await _uow.Connection.QueryAsync<dynamic>(
                "SELECT id FROM aircraft;", transaction: _uow.Transaction)).ToList();

            if (!templates.Any() || !aircrafts.Any())
                throw new InvalidOperationException("Não há templates ou aeronaves cadastradas.");

            var today = DateTime.UtcNow.Date;
            var targetDate = today.AddDays(daysAhead);

            const string lastDateSql = "SELECT MAX(DATE(departure_at)) FROM flight_instance;";
            var lastGeneratedDate = await _uow.Connection.ExecuteScalarAsync<DateTime?>(lastDateSql, transaction: _uow.Transaction);

            var startDate = lastGeneratedDate?.AddDays(1) ?? today;
            if (startDate > targetDate)
            {
                Console.WriteLine("✅ Agenda já atualizada até a data limite.");
                return;
            }

            var dateRange = Enumerable.Range(0, (targetDate - startDate).Days + 1)
                .Select(offset => startDate.AddDays(offset));

            var newFlights = dateRange
                .SelectMany(date =>
                    templates.SelectMany(template =>
                        Enumerable.Range(0, 8).Select(i =>
                        {
                            var departure = date.AddHours(i * 3);
                            var arrival = departure.AddHours(1);
                            var aircraft = aircrafts[i % aircrafts.Count];

                            return new FlightInstance
                            {
                                FlightTemplateId = (int)template.id,
                                AircraftId = (int)aircraft.id,
                                DepartureAt = departure,
                                ArrivalAt = arrival
                            };
                        })
                    )
                ).ToList();

            foreach (var flight in newFlights)
                await flightRepo.AddAsync(flight);

            await _uow.CommitAsync();

            Console.WriteLine($"✈️ {newFlights.Count} voos gerados de {startDate:dd/MM} até {targetDate:dd/MM}");
        }
        
        // ================================================================
        // 3️⃣ Limpeza de voos antigos
        // ================================================================
        public async Task PurgeOldFlightsAsync()
        {
            const string sql = @"DELETE FROM flight_instance WHERE DATE(departure_at) < CURRENT_DATE;";
            await _uow.Connection.ExecuteAsync(sql, transaction: _uow.Transaction);
        }
    }
}
