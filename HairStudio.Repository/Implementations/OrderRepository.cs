using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairStudio.Repository.Implementations
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(HairStudioContext context) : base(context)
        {
        }

        public async Task<OrderStatus?> GetOrderStatusByIdAsync(short orderStatusId)
        {
            return await _context.OrderStatuses.FindAsync(orderStatusId);
        }

        public IQueryable<Order> GetOrders()
        {
            return GetAll().Include(o => o.User)
                           .Include(o => o.OrderStatus)
                           .Include(o => o.PaymentStatus)
                           .Include(o => o.OrderItems)
                           .ThenInclude(oi => oi.Product);
        }
    }
}
