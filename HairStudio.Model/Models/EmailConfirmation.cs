using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class EmailConfirmation
    {
        public Guid EmailConfirmationId { get; set; }
        public short UserId { get; set; }
        public string ConfirmationCode { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
