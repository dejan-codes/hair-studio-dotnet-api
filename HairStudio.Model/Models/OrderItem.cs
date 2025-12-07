using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class OrderItem
    {
        public int OrderId { get; set; }
        public short ProductId { get; set; }
        public short Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
