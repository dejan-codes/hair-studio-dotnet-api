using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class ProductType
    {
        public ProductType()
        {
            Products = new HashSet<Product>();
        }

        public short ProductTypeId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
