using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisandAirlines.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string Method { get; set; } = "PIX";
        public decimal Amount { get; set; }
        public string Status { get; set; } = "CONFIRMED";
        public DateTime? PaidAt { get; set; }
        public string? ConfirmationCode { get; set; }
    }
}
