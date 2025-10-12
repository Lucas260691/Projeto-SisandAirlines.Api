using Dapper;
using System.Data;
using SisandAirlines.Domain.Entities;
using SisandAirlines.Domain.Interfaces;

namespace SisandAirlines.Infrastructure.Repositories
{
    public class FlightRepository : BaseRepository<FlightInstance>, IFlightRepository
    {
        public FlightRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base(connection, transaction) { }

        public override async Task<IEnumerable<FlightInstance>> GetAllAsync()
        {
            var sql = "SELECT * FROM flight_instance;";
            return await _connection.QueryAsync<FlightInstance>(sql, transaction: _transaction);
        }

        public override async Task<FlightInstance?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM flight_instance WHERE id = @id;";
            return await _connection.QueryFirstOrDefaultAsync<FlightInstance>(sql, new { id }, transaction: _transaction);
        }

        public override async Task AddAsync(FlightInstance entity)
        {
            var sql = @"INSERT INTO flight_instance (flight_template_id, aircraft_id, departure_at, arrival_at)
                        VALUES (@FlightTemplateId, @AircraftId, @DepartureAt, @ArrivalAt)";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }

        public override async Task UpdateAsync(FlightInstance entity)
        {
            var sql = @"UPDATE flight_instance 
                        SET aircraft_id = @AircraftId, departure_at = @DepartureAt, arrival_at = @ArrivalAt 
                        WHERE id = @Id";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }

        public override async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM flight_instance WHERE id = @id;";
            await _connection.ExecuteAsync(sql, new { id }, transaction: _transaction);
        }

        public async Task<IEnumerable<dynamic>> GetAvailableFlightsAsync(DateTime date)
        {
            var sql = @"
                SELECT fi.id, ft.origin, ft.destination, fi.departure_at, fi.arrival_at, ft.base_fare, a.model
                FROM flight_instance fi
                INNER JOIN flight_template ft ON fi.flight_template_id = ft.id
                INNER JOIN aircraft a ON fi.aircraft_id = a.id
                WHERE DATE(fi.departure_at) = @date;";

            return await _connection.QueryAsync(sql, new { date }, transaction: _transaction);
        }
    }
}

