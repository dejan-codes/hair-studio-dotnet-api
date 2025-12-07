using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairStudio.Repository.Implementations
{
    public class ProductTypeRepository : BaseRepository<ProductType>, IProductTypeRepository
    {
        public ProductTypeRepository(HairStudioContext context) : base(context)
        {
        }

        public async Task<(ProductType? ProductType, bool HasActiveProducts)> GetProductTypeWithCheckAsync(short productTypeId)
        {
            var result = await GetAll()
                .Where(productType => productType.ProductTypeId == productTypeId && productType.IsActive)
                .Select(productType => new
                {
                    ProductType = productType,
                    HasActiveProducts = productType.Products.Any(p => p.IsActive)
                })
                .FirstOrDefaultAsync();

            return result == null ? (null, false) : (result.ProductType, result.HasActiveProducts);
        }

        public async Task<bool> ProductTypeExistsByNameAsync(string productTypeName)
        {
            return await GetAll()
                .AnyAsync(b => b.Name == productTypeName);
        }
    }
}
