using System;
using System.Collections.Generic;

namespace HairStudio.Model.Models
{
    public partial class User
    {
        public User()
        {
            EmailConfirmations = new HashSet<EmailConfirmation>();
            Messages = new HashSet<Message>();
            Orders = new HashSet<Order>();
            PasswordResetTokens = new HashSet<PasswordResetToken>();
            Products = new HashSet<Product>();
            ReservationClientUsers = new HashSet<Reservation>();
            ReservationEmployees = new HashSet<Reservation>();
            Services = new HashSet<Service>();
            WorkHours = new HashSet<WorkHour>();
            Roles = new HashSet<Role>();
        }

        public short UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public string? Bio { get; set; }
        public string PasswordHash { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
        public byte[]? Image { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<EmailConfirmation> EmailConfirmations { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Reservation> ReservationClientUsers { get; set; }
        public virtual ICollection<Reservation> ReservationEmployees { get; set; }
        public virtual ICollection<Service> Services { get; set; }
        public virtual ICollection<WorkHour> WorkHours { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
