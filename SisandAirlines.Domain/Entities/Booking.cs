using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisandAirlines.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int FlightInstanceId { get; set; }
        public string SeatClass { get; set; } = string.Empty;
        public string? SeatNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
