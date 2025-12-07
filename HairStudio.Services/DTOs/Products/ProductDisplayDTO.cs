namespace HairStudio.Services.DTOs.Products
{
    public class ProductDisplayDTO
    {
        public short ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public byte[] Image { get; set; } = null!;
        public short BrandId { get; set; }
        public short ProductTypeId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public byte SequenceNumber { get; set; }
    }
}
