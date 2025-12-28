using HairStudio.Services.Common;
using HairStudio.Services.DTOs.ProductTypes;

namespace HairStudio.Services.Interfaces
{
    public interface IProductTypeService
    {
        Task<Result<object>> GetPagedProductTypesAsync(int page, int rowsPerPage);
        Task<Result> CreateProductTypeAsync(ProductTypeCreateDTO productTypeCreateDTO);
        Task<Result> UpdateProductTypeAsync(short productTypeId, ProductTypeUpdateDTO productTypeUpdateDTO);
        Task<Result> DeleteProductTypeAsync(short productTypeId);
        Task<Result<IEnumerable<ProductTypeDTO>>> GetProductTypesForDropdownAsync();
    }
}
