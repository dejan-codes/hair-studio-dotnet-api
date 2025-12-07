using HairStudio.Model.Models;

namespace HairStudio.Repository.Interfaces
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        IQueryable<Product> GetActiveProducts();
    }
}
