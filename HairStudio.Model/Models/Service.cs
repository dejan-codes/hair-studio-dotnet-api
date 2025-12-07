using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class Service
    {
        public Service()
        {
            Reservations = new HashSet<Reservation>();
        }

        public short ServiceId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int DurationMinutes { get; set; }
        public byte[] Image { get; set; } = null!;
        public byte GenderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public short? CreatedByUserId { get; set; }
        public byte SequenceNumber { get; set; }
        public bool IsActive { get; set; }

        public virtual User? CreatedByUser { get; set; }
        public virtual Gender Gender { get; set; } = null!;
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
