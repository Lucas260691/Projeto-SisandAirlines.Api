using SisandAirlines.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisandAirlines.Domain.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetByCustomerAsync(int customerId);
        Task<bool> SeatAlreadyBookedAsync(int flightInstanceId, int seatId);
    }
}
