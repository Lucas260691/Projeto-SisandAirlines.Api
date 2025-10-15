using Dapper;
using System.Data;
using SisandAirlines.Domain.Entities;
using SisandAirlines.Domain.Interfaces;

namespace SisandAirlines.Infrastructure.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IDbConnection connection, IDbTransaction? transaction = null)
            : base(connection, transaction) { }

        public override async Task<IEnumerable<Customer>> GetAllAsync()
        {
            const string sql = @"
                SELECT 
                    id, 
                    full_name AS FullName, 
                    email AS Email, 
                    cpf AS Cpf, 
                    password_hash AS PasswordHash, 
                    birth_date AS BirthDate, 
                    created_at AS CreatedAt
                FROM customer;
            ";
            return await _connection.QueryAsync<Customer>(sql, transaction: _transaction);
        }

        public override async Task<Customer?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    id, 
                    full_name AS FullName, 
                    email AS Email, 
                    cpf AS Cpf, 
                    password_hash AS PasswordHash, 
                    birth_date AS BirthDate, 
                    created_at AS CreatedAt
                FROM customer 
                WHERE id = @id;
            ";
            return await _connection.QueryFirstOrDefaultAsync<Customer>(sql, new { id }, transaction: _transaction);
        }

        public override async Task AddAsync(Customer entity)
        {
            const string sql = @"
                INSERT INTO customer (full_name, email, cpf, password_hash, birth_date)
                VALUES (@FullName, @Email, @Cpf, @PasswordHash, @BirthDate);
            ";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }

        public override async Task UpdateAsync(Customer entity)
        {
            const string sql = @"
                UPDATE customer 
                SET full_name = @FullName, email = @Email
                WHERE id = @Id;
            ";
            await _connection.ExecuteAsync(sql, entity, transaction: _transaction);
        }

        public override async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM customer WHERE id = @id;";
            await _connection.ExecuteAsync(sql, new { id }, transaction: _transaction);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            const string sql = @"
                SELECT 
                    id, 
                    full_name AS FullName, 
                    email AS Email, 
                    cpf AS Cpf, 
                    password_hash AS PasswordHash, 
                    birth_date AS BirthDate, 
                    created_at AS CreatedAt
                FROM customer 
                WHERE email = @Email;
            ";
            return await _connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Email = email }, transaction: _transaction);
        }
    }
}
