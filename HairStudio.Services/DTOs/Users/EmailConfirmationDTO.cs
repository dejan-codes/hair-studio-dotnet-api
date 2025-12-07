using System.ComponentModel.DataAnnotations;

namespace HairStudio.Services.DTOs.Users
{
    public class EmailConfirmationDTO
    {
        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Code must be between 2 and 255 characters.")]
        public string Code { get; set; } = string.Empty;
    }
}
