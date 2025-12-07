using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class Reservation
    {
        public short ReservationId { get; set; }
        public short ServiceId { get; set; }
        public short? ClientUserId { get; set; }
        public short? ClientCustomerId { get; set; }
        public short EmployeeId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string? Note { get; set; }
        public bool IsActive { get; set; }

        public virtual Customer? ClientCustomer { get; set; }
        public virtual User? ClientUser { get; set; }
        public virtual User Employee { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
    }
}
