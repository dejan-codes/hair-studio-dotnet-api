using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairStudio.Repository.Implementations
{
    public class BrandRepository : BaseRepository<Brand>, IBrandRepository
    {
        public BrandRepository(HairStudioContext context) : base(context)
        {
        }

        public async Task<(Brand? Brand, bool HasActiveProducts)> GetBrandWithCheckAsync(short brandId)
        {
            var result = await GetAll()
                .Where(b => b.BrandId == brandId && b.IsActive)
                .Select(b => new
                {
                    Brand = b,
                    HasActiveProducts = b.Products.Any(p => p.IsActive)
                })
                .FirstOrDefaultAsync();

            return result == null ? (null, false) : (result.Brand, result.HasActiveProducts);
        }

        public async Task<bool> BrandExistsByNameAsync(string brandName)
        {
            return await GetAll()
                .AnyAsync(b => b.Name == brandName);
        }
    }
}
