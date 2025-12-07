using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public short ProductId { get; set; }
        public short BrandId { get; set; }
        public short ProductTypeId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public byte[] Image { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public short? CreatedByUserId { get; set; }
        public byte SequenceNumber { get; set; }
        public int NumberOfPurchases { get; set; }
        public bool IsActive { get; set; }

        public virtual Brand Brand { get; set; } = null!;
        public virtual User? CreatedByUser { get; set; }
        public virtual ProductType ProductType { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
