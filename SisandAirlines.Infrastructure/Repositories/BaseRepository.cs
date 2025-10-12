using Dapper;
using System.Data;
using SisandAirlines.Domain.Interfaces;

namespace SisandAirlines.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly IDbConnection _connection;
        protected readonly IDbTransaction? _transaction;

        protected BaseRepository(IDbConnection connection, IDbTransaction? transaction = null)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public abstract Task<IEnumerable<T>> GetAllAsync();
        public abstract Task<T?> GetByIdAsync(int id);
        public abstract Task AddAsync(T entity);
        public abstract Task UpdateAsync(T entity);
        public abstract Task DeleteAsync(int id);
    }
}
