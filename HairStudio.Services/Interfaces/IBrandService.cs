using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Brands;

namespace HairStudio.Services.Interfaces
{
    public interface IBrandService
    {
        Task<Result<object>> GetPagedBrandsAsync(int page, int rowsPerPage);
        Task<Result> CreateBrandAsync(BrandCreateDTO brandCreateDTO, short tokenUserId);
        Task<Result> UpdateBrandAsync(short brandId, BrandUpdateDTO brandUpdateDTO, short tokenUserId);
        Task<Result> DeleteBrandAsync(short brandId, short tokenUserId);
        Task<Result<IEnumerable<BrandDTO>>> GetBrandsForDropdownAsync();
    }
}
