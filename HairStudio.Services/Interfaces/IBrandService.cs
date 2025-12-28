using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Brands;

namespace HairStudio.Services.Interfaces
{
    public interface IBrandService
    {
        Task<Result<object>> GetPagedBrandsAsync(int page, int rowsPerPage);
        Task<Result> CreateBrandAsync(BrandCreateDTO brandCreateDTO);
        Task<Result> UpdateBrandAsync(short brandId, BrandUpdateDTO brandUpdateDTO);
        Task<Result> DeleteBrandAsync(short brandId);
        Task<Result<IEnumerable<BrandDTO>>> GetBrandsForDropdownAsync();
    }
}
