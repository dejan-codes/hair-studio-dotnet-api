using System.ComponentModel.DataAnnotations;

namespace HairStudio.Services.DTOs.Users
{
    public class ResendConfirmationCodeDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Email can't be longer than 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
    }
}
