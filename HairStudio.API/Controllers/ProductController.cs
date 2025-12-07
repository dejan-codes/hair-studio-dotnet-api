using HairStudio.Services.DTOs.Products;
using HairStudio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HairStudio.Services.Common;
using HairStudio.Services.Errors;
using HairStudio.API.Infrastructure;

namespace HairStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ICurrentUserContext currentUserContext, ILogger<ProductController> logger)
        {
            _productService = productService;
            _currentUserContext = currentUserContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync(int page, int rowsPerPage, string? search, int? brand, int? type, int minPrice = 0, int maxPrice = 9999, string? sortOrder = "asc")
        {
            var result = await _productService.GetProductsAsync(page, rowsPerPage, search, brand, type, minPrice, maxPrice, sortOrder);
            return result.ToActionResult();
        }

        [HttpGet("popularity")]
        public async Task<IActionResult> GetMostPopularProductsAsync()
        {
            var result = await _productService.GetMostPopularProductsAsync();
            return result.ToActionResult();
        }

        [Authorize(Roles = "User,Employee,Administrator")]
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrdersAsync(int page, int rowsPerPage)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _productService.GetOrdersAsync(tokenUserId.Value, page, rowsPerPage);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromForm] ProductCreateDTO productCreateDTO)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _productService.CreateProductAsync(productCreateDTO, tokenUserId.Value);
            return result.ToActionResult();
        }

        [Authorize(Roles = "User,Employee,Administrator")]
        [HttpPost("buy")]
        public async Task<IActionResult> BuyProductsAsync([FromBody] List<BuyProductDTO> buyProductDTOList)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _productService.BuyProductsAsync(buyProductDTOList, tokenUserId.Value);
            return result.ToActionResult();
        }

        [HttpGet("session/{sessionId}")]
        public IActionResult GetSessionDetails(string sessionId)
        {
            try
            {
                var details = _productService.GetSessionDetails(sessionId);
                return Ok(details);
            }
            catch (Stripe.StripeException ex)
            {
                _logger.LogError(ex, "Failed to retrieve Stripe session details for sessionId: {SessionId}", sessionId);
                return BadRequest(new { error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleStripeWebhookAsync()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            if (!Request.Headers.TryGetValue("Stripe-Signature", out var signatureValues))
                return Result.Failure(ValidationErrors.InvalidData).ToActionResult();

            var stripeSignature = signatureValues.ToString();

            var result = await _productService.HandleStripeWebhookAsync(json, stripeSignature);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductAsync(short productId, [FromForm] ProductUpdateDTO productUpdateDTO)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _productService.UpdateProductAsync(productId, productUpdateDTO, tokenUserId.Value);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("change-order-status")]
        public async Task<IActionResult> ChangeOrderStatusAsync(int orderId, short status)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _productService.ChangeOrderStatusAsync(orderId, status, tokenUserId.Value);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProductAsync(short productId)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _productService.DeleteProductAsync(productId, tokenUserId.Value);
            return result.ToActionResult();
        }
    }
}
