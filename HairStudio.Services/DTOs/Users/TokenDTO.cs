using System.ComponentModel.DataAnnotations;

namespace HairStudio.Services.DTOs.Users
{
    public class TokenDTO
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
