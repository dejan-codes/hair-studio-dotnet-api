using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class Brand
    {
        public Brand()
        {
            Products = new HashSet<Product>();
        }

        public short BrandId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
