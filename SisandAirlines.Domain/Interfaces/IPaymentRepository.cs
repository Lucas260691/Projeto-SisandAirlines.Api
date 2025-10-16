using SisandAirlines.Domain.Entities;

namespace SisandAirlines.Domain.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetByCustomerAsync(int customerId);
        Task<Payment?> GetByBookingIdAsync(int bookingId);
    }
}
