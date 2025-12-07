using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class PaymentStatus
    {
        public PaymentStatus()
        {
            Orders = new HashSet<Order>();
        }

        public short PaymentStatusId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; }
    }
}
