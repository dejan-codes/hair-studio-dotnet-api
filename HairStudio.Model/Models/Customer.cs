using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Reservations = new HashSet<Reservation>();
        }

        public short CustomerId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
