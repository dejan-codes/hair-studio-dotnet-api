using HairStudio.Services.Common;
using HairStudio.Services.DTOs.ProductTypes;

namespace HairStudio.Services.Interfaces
{
    public interface IProductTypeService
    {
        Task<Result<object>> GetPagedProductTypesAsync(int page, int rowsPerPage);
        Task<Result> CreateProductTypeAsync(ProductTypeCreateDTO productTypeCreateDTO, short tokenUserId);
        Task<Result> UpdateProductTypeAsync(short productTypeId, ProductTypeUpdateDTO productTypeUpdateDTO, short tokenUserId);
        Task<Result> DeleteProductTypeAsync(short productTypeId, short tokenUserId);
        Task<Result<IEnumerable<ProductTypeDTO>>> GetProductTypesForDropdownAsync();
    }
}
