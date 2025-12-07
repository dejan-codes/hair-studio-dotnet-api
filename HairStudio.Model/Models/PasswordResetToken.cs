using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class PasswordResetToken
    {
        public int PasswordResetTokenId { get; set; }
        public short UserId { get; set; }
        public string ResetToken { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
