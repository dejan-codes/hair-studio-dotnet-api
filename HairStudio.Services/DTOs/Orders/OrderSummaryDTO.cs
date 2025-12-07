namespace HairStudio.Services.DTOs.Orders
{
    public class OrderSummaryDTO
    {
        public int OrderId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int OrderStatusId { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
        public List<OrderItemDetailDTO> OrdersDTO { get; set; } = new List<OrderItemDetailDTO>();
    }
}
