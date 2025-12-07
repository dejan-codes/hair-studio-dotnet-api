using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class Message
    {
        public int MessageId { get; set; }
        public short UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
