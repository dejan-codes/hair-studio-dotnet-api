using System.ComponentModel.DataAnnotations;

namespace HairStudio.Services.DTOs.Products
{
    public class BuyProductDTO
    {
        [Required]
        public short ProductId { get; set; }

        [Required]
        [Range(0, 255, ErrorMessage = "Quantity must be between 0 and 255.")]
        public short Quantity { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
    }
}
