using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int OrderId { get; set; }
        public short UserId { get; set; }
        public short OrderStatusId { get; set; }
        public short PaymentStatusId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }

        public virtual OrderStatus OrderStatus { get; set; } = null!;
        public virtual PaymentStatus PaymentStatus { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
