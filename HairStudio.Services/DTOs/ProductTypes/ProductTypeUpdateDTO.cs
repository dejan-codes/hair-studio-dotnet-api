using System.ComponentModel.DataAnnotations;

namespace HairStudio.Services.DTOs.ProductTypes
{
    public class ProductTypeUpdateDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}
