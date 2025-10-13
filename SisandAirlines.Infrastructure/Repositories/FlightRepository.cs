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
            const string sql = "SELECT * FROM flight_instance;";
            return await _connection.QueryAsync<FlightInstance>(sql, transaction: _transaction);
        }

        public override async Task<FlightInstance?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM flight_instance WHERE id = @id;";
            return await _connection.QueryFirstOrDefaultAsync<FlightInstance>(sql, new { id }, transaction: _transaction);
        }

        public override async Task AddAsync(FlightInstance entity)
        {
            const string sql = @"
                INSERT INTO flight_instance (flight_template_id, aircraft_id, departure_at, arrival_at)
                VALUES (@FlightTemplateId, @AircraftId, @DepartureAt, @ArrivalAt)";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }

        public override async Task UpdateAsync(FlightInstance entity)
        {
            const string sql = @"
                UPDATE flight_instance 
                SET aircraft_id = @AircraftId, departure_at = @DepartureAt, arrival_at = @ArrivalAt 
                WHERE id = @Id";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }

        public override async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM flight_instance WHERE id = @id;";
            await _connection.ExecuteAsync(sql, new { id }, transaction: _transaction);
        }

        public async Task<IEnumerable<FlightInstance>> GetAvailableFlightsAsync(DateTime date)
        {
            const string sql = @"
                SELECT 
                    fi.id AS Id,
                    fi.flight_template_id AS FlightTemplateId,
                    fi.aircraft_id AS AircraftId,
                    fi.departure_at AS DepartureAt,
                    fi.arrival_at AS ArrivalAt,
                    ft.origin AS Origin,
                    ft.destination AS Destination,
                    a.model AS AircraftModel,
                    a.code AS AircraftCode,
                    f.""class"" AS FareClass,
                    f.amount AS BaseFare
                FROM flight_instance fi
                INNER JOIN flight_template ft ON fi.flight_template_id = ft.id
                INNER JOIN aircraft a ON fi.aircraft_id = a.id
                CROSS JOIN fare f
                WHERE DATE(fi.departure_at) = @date
                ORDER BY fi.departure_at, f.class;
            ";

            return await _connection.QueryAsync<FlightInstance>(
                sql,
                new { date },
                transaction: _transaction
            );
        }
    }
}
