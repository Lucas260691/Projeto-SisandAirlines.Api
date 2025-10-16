using Dapper;
using System.Data;
using SisandAirlines.Domain.Entities;
using SisandAirlines.Domain.Interfaces;

namespace SisandAirlines.Infrastructure.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base(connection, transaction) { }

        public override async Task<IEnumerable<Payment>> GetAllAsync()
        {
            const string sql = @"
                SELECT 
                    id,
                    booking_id     AS BookingId,
                    amount         AS Amount,
                    method         AS Method,
                    paid_at        AS PaidAt,
                    confirmation_code AS ConfirmationCode
                FROM payment;";
            return await _connection.QueryAsync<Payment>(sql, transaction: _transaction);
        }

        public override async Task<Payment?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    id,
                    booking_id     AS BookingId,
                    amount         AS Amount,
                    method         AS Method,
                    paid_at        AS PaidAt,
                    confirmation_code AS ConfirmationCode
                FROM payment
                WHERE id = @id;";
            return await _connection.QueryFirstOrDefaultAsync<Payment>(sql, new { id }, transaction: _transaction);
        }

        public override async Task AddAsync(Payment entity)
        {
            const string sql = @"
                INSERT INTO payment (booking_id, amount, method, paid_at, confirmation_code)
                VALUES (@BookingId, @Amount, @Method, @PaidAt, @ConfirmationCode)
                RETURNING id;";
            var newId = await _connection.ExecuteScalarAsync<int>(sql, entity, transaction: _transaction);
            entity.Id = newId;
        }

        public override async Task UpdateAsync(Payment entity)
        {
            const string sql = @"
                UPDATE payment
                SET amount = @Amount,
                    method = @Method,
                    paid_at = @PaidAt,
                    confirmation_code = @ConfirmationCode
                WHERE id = @Id;";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }

        public override async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM payment WHERE id = @id;";
            await _connection.ExecuteAsync(sql, new { id }, transaction: _transaction);
        }

        public async Task<IEnumerable<Payment>> GetByCustomerAsync(int customerId)
        {
            const string sql = @"
                SELECT 
                    p.id,
                    p.booking_id     AS BookingId,
                    p.amount         AS Amount,
                    p.method         AS Method,
                    p.paid_at        AS PaidAt,
                    p.confirmation_code AS ConfirmationCode
                FROM payment p
                INNER JOIN booking b ON p.booking_id = b.id
                WHERE b.customer_id = @customerId;";
            return await _connection.QueryAsync<Payment>(sql, new { customerId }, transaction: _transaction);
        }

        public async Task<Payment?> GetByBookingIdAsync(int bookingId)
        {
            const string sql = @"
                SELECT 
                    id,
                    booking_id     AS BookingId,
                    amount         AS Amount,
                    method         AS Method,
                    paid_at        AS PaidAt,
                    confirmation_code AS ConfirmationCode
                FROM payment
                WHERE booking_id = @bookingId;";
            return await _connection.QueryFirstOrDefaultAsync<Payment>(sql, new { bookingId }, transaction: _transaction);
        }
    }
}
