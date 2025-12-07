using HairStudio.Services.Common;

namespace HairStudio.Services.Errors
{
    public static class OrderErrors
    {
        public static readonly Error OrderNotFound = new Error(
            "Order.OrderNotFound", "Order not found.");

        public static readonly Error OrderStatusNotFound = new Error(
            "Order.OrderStatusNotFound", "Order status not found.");
    }
}
