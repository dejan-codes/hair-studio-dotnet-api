using System.ComponentModel.DataAnnotations;

namespace HairStudio.Services.DTOs.Users
{
    public partial class PasswordUpdateDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters.")]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
