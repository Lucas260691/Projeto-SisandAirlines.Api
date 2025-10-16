using System;
using System.Data;
using System.Threading.Tasks;

namespace SisandAirlines.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IFlightRepository Flights { get; }
        IBookingRepository Bookings { get; }
        ICustomerRepository Customers { get; }
        IPaymentRepository Payments { get; }

        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }

        //Task InitializeAsync();

        Task CommitAsync();
        Task RollbackAsync();
    }
}
