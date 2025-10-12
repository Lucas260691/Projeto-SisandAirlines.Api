using System.Data;
using Npgsql;
using SisandAirlines.Domain.Interfaces;
using SisandAirlines.Infrastructure.Repositories;

namespace SisandAirlines.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private IDbTransaction? _transaction;

        public IFlightRepository Flights { get; }

        public UnitOfWork(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
            Flights = new FlightRepository(_connection, _transaction);
        }

        public async Task CommitAsync()
        {
            _transaction?.Commit();
            _transaction = _connection.BeginTransaction();
        }

        public async Task RollbackAsync()
        {
            _transaction?.Rollback();
            _transaction = _connection.BeginTransaction();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection.Dispose();
        }
    }
}
