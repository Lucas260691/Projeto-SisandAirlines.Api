using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisandAirlines.Domain.Entities
{
    public class Aircraft
    {
        public int Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}
