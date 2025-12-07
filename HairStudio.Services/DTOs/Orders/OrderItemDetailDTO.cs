namespace HairStudio.Services.DTOs.Orders
{
    public class OrderItemDetailDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
