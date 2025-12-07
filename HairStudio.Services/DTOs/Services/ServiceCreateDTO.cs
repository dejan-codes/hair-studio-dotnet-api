using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HairStudio.Services.DTOs.Services
{
    public class ServiceCreateDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Service name must be between 2 and 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description can't be longer than 2000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, 1000000, ErrorMessage = "Discount must be positive.")]
        public decimal? Discount { get; set; }

        [Required]
        [Range(1, 480, ErrorMessage = "Duration must be between 1 and 480 minutes.")]
        public int DurationMinutes { get; set; }

        [Range(1, 2, ErrorMessage = "Invalid gender.")]
        public byte GenderId { get; set; }

        [Required]
        public IFormFile Image { get; set; } = null!;

        [Required]
        [Range(0, 255, ErrorMessage = "Sequence number must be between 0 and 255.")]
        public byte SequenceNumber { get; set; }
    }
}
