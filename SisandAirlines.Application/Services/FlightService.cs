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
        // 1️⃣ Consulta voos disponíveis
        // ================================================================
        public async Task<IEnumerable<FlightAvailableDto>> GetAvailableFlightsAsync(DateTime date)
        {
            if (date.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Não é possível consultar voos para datas passadas.");

            var flights = await _uow.Flights.GetAvailableFlightsAsync(date);

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

        // ================================================================
        // 2️⃣ Geração contínua de voos (janela de 60 dias)
        // ================================================================
        public async Task GenerateFutureFlightsAsync(int daysAhead = 60)
        {
            var flightRepo = _uow.Flights;

            // Recupera templates e aeronaves
            var templates = (await _uow.Connection.QueryAsync<dynamic>(
                "SELECT id FROM flight_template;", transaction: _uow.Transaction)).ToList();

            var aircrafts = (await _uow.Connection.QueryAsync<dynamic>(
                "SELECT id FROM aircraft;", transaction: _uow.Transaction)).ToList();

            if (!templates.Any() || !aircrafts.Any())
                throw new InvalidOperationException("Não há templates ou aeronaves cadastradas.");

            // Define janela alvo (hoje até +60 dias)
            var today = DateTime.UtcNow.Date;
            var targetDate = today.AddDays(daysAhead);

            // Verifica até onde já existem voos gerados
            const string lastDateSql = "SELECT MAX(DATE(departure_at)) FROM flight_instance;";
            var lastGeneratedDate = await _uow.Connection.ExecuteScalarAsync<DateTime?>(lastDateSql, transaction: _uow.Transaction);

            var startDate = lastGeneratedDate?.AddDays(1) ?? today;
            if (startDate > targetDate)
            {
                Console.WriteLine("✅ Agenda já atualizada até a data limite.");
                return;
            }

            // Gera os voos que faltam (de forma declarativa)
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
