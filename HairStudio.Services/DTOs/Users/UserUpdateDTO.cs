using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HairStudio.Services.DTOs.Users
{
    public partial class UserUpdateDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Phone number can't be longer than 50 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Bio can't be longer than 50 characters.")]
        public string Bio { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Email can't be longer than 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "At least one role is required.")]
        public List<int> Roles { get; set; } = new();

        public IFormFile? Image { get; set; }
    }
}
