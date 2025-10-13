using Dapper;
using System.Data;
using SisandAirlines.Domain.Entities;
using SisandAirlines.Domain.Interfaces;

namespace SisandAirlines.Infrastructure.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base(connection, transaction) { }

        public override async Task<IEnumerable<Booking>> GetAllAsync()
        {
            const string sql = "SELECT * FROM booking;";
            return await _connection.QueryAsync<Booking>(sql, transaction: _transaction);
        }

        public override async Task<Booking?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM booking WHERE id = @id;";
            return await _connection.QueryFirstOrDefaultAsync<Booking>(sql, new { id }, transaction: _transaction);
        }

        public override async Task AddAsync(Booking entity)
        {
            const string sql = @"
                INSERT INTO booking (customer_id, flight_instance_id, seat_id, fare_id, payment_status)
                VALUES (@CustomerId, @FlightInstanceId, @SeatId, @FareId, @PaymentStatus);
            ";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }

        public override async Task UpdateAsync(Booking entity)
        {
            const string sql = @"
                UPDATE booking 
                SET payment_status = @PaymentStatus
                WHERE id = @Id;
            ";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }

        public override async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM booking WHERE id = @id;";
            await _connection.ExecuteAsync(sql, new { id }, transaction: _transaction);
        }

        public async Task<IEnumerable<Booking>> GetByCustomerAsync(int customerId)
        {
            const string sql = "SELECT * FROM booking WHERE customer_id = @customerId;";
            return await _connection.QueryAsync<Booking>(sql, new { customerId }, transaction: _transaction);
        }

        public async Task<bool> SeatAlreadyBookedAsync(int flightInstanceId, int seatId)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM booking
                WHERE flight_instance_id = @flightInstanceId
                  AND seat_id = @seatId
                  AND payment_status != 'CANCELLED';
            ";

            var count = await _connection.ExecuteScalarAsync<int>(sql, new { flightInstanceId, seatId }, transaction: _transaction);
            return count > 0;
        }
    }
}
