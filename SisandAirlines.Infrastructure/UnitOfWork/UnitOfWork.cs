using System.Data;
using Npgsql;
using SisandAirlines.Domain.Interfaces;
using SisandAirlines.Infrastructure.Repositories;

namespace SisandAirlines.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NpgsqlConnection _connection;
        private NpgsqlTransaction? _transaction;
        private bool _disposed;
        public IFlightRepository Flights { get; private set; } = null!;
        public IBookingRepository Bookings { get; private set; } = null!;

        public UnitOfWork(string connectionString)
        {
            Console.WriteLine("🟢 Criando UnitOfWork...");

            _connection = new NpgsqlConnection(connectionString);

            _connection.Open();
            _transaction = _connection.BeginTransaction();

            Flights = new FlightRepository(_connection, _transaction);
            Bookings = new BookingRepository(_connection, _transaction);

            Console.WriteLine($"✅ Repositórios inicializados: Flights={Flights != null}, Bookings={Bookings != null}");
        }
        
        //public async Task InitializeAsync()
        //{
        //    await _connection.OpenAsync();
        //    _transaction = await _connection.BeginTransactionAsync();

        //    Flights = new FlightRepository(_connection, _transaction);
        //    Bookings = new BookingRepository(_connection, _transaction);
        //}

        public async Task CommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction not initialized.");

            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                _transaction = await _connection.BeginTransactionAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
                return;

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                _transaction = await _connection.BeginTransactionAsync();
            }
        }

        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            if (_transaction != null)
                await _transaction.DisposeAsync();

            await _connection.CloseAsync();
            await _connection.DisposeAsync();

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
