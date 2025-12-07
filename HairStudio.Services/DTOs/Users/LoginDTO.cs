using System.ComponentModel.DataAnnotations;

namespace HairStudio.Services.DTOs.Users
{
    public class LoginDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Email can't be longer than 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
