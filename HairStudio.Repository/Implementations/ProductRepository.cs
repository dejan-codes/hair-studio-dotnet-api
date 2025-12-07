using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairStudio.Repository.Implementations
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(HairStudioContext context) : base(context)
        {
        }

        public IQueryable<Product> GetActiveProducts()
        {
            return GetAll()
                .Include(p => p.Brand)
                .Include(p => p.ProductType)
                .Where(p => p.IsActive);
        }
    }
}
