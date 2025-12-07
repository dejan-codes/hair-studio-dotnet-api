using HairStudio.Model.Models;

namespace HairStudio.Repository.Interfaces
{
    public interface IProductTypeRepository : IRepositoryBase<ProductType>
    {
        Task<(ProductType? ProductType, bool HasActiveProducts)> GetProductTypeWithCheckAsync(short productTypeId);
        Task<bool> ProductTypeExistsByNameAsync(string productTypeName);
    }
}
