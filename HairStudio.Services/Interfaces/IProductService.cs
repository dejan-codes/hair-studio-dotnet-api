using HairStudio.Model.Models;
using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Products;

namespace HairStudio.Services.Interfaces
{
    public interface IProductService
    {
        Task<Result<object>> GetProductsAsync(int page, int rowsPerPage, string? search, int? brand, int? type, int minPrice, int maxPrice, string? sortOrder);
        Task<Result<object>> GetMostPopularProductsAsync();
        Task<Result<object>> GetOrdersAsync(int page, int rowsPerPage);
        Task<Result> CreateProductAsync(ProductCreateDTO productCreateDTO);
        Task<Result<UrlResponseDTO>> BuyProductsAsync(List<BuyProductDTO> buyProductDTOList);
        object GetSessionDetails(string sessionId);
        Task<Result> HandleStripeWebhookAsync(string json, string stripeSignature);
        Task<Result> UpdateProductAsync(short productId, ProductUpdateDTO productUpdateDTO);
        Task<Result> ChangeOrderStatusAsync(int orderId, short status);
        Task<Result> DeleteProductAsync(short productId);
    }
}
