using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class Gender
    {
        public Gender()
        {
            Services = new HashSet<Service>();
        }

        public byte GenderId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Service> Services { get; set; }
    }
}
