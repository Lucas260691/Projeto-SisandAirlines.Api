using System;
using System.Threading.Tasks;

namespace SisandAirlines.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IFlightRepository Flights { get; }
        IBookingRepository Bookings { get; }
        ICustomerRepository Customers { get; }

        //Task InitializeAsync();

        Task CommitAsync();
        Task RollbackAsync();
    }
}
