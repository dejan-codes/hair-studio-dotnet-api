using System.ComponentModel.DataAnnotations;

namespace HairStudio.Services.DTOs.Users
{
    public class UserRegistrationDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Email can't be longer than 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Phone number can't be longer than 50 characters.")]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 255 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
