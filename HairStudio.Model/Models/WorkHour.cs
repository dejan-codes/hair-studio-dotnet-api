using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class WorkHour
    {
        public short WorkHourId { get; set; }
        public short UserId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
