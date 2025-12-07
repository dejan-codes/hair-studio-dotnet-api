using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HairStudio.Services.DTOs.Products
{
    public class ProductCreateDTO
    {
        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 255 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description can't be longer than 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }

        [Required]
        [Range(0, 255, ErrorMessage = "Sequence number must be between 0 and 255.")]
        public byte SequenceNumber { get; set; }

        [Required]
        public short BrandId { get; set; }

        [Required]
        public short ProductTypeId { get; set; }

        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
