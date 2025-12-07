using HairStudio.Model.Models;

namespace HairStudio.Repository.Interfaces
{
    public interface IBrandRepository : IRepositoryBase<Brand>
    {
        Task<(Brand? Brand, bool HasActiveProducts)> GetBrandWithCheckAsync(short brandId);
        Task<bool> BrandExistsByNameAsync(string brandName);
    }
}
