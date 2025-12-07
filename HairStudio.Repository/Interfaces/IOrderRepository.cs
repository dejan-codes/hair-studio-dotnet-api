using HairStudio.Model.Models;

namespace HairStudio.Repository.Interfaces
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        Task<OrderStatus?> GetOrderStatusByIdAsync(short orderStatusId);
        IQueryable<Order> GetOrders();
    }
}
