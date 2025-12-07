using HairStudio.API.Infrastructure;
using HairStudio.Services.Common;
using HairStudio.Services.DTOs.ProductTypes;
using HairStudio.Services.Errors;
using HairStudio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;
        private readonly ICurrentUserContext _currentUserContext;

        public ProductTypeController(IProductTypeService productTypeService, ICurrentUserContext currentUserContext)
        {
            _productTypeService = productTypeService;
            _currentUserContext = currentUserContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedProductTypesAsync(int page, int rowsPerPage)
        {
            var result = await _productTypeService.GetPagedProductTypesAsync(page, rowsPerPage);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateProductTypeAsync([FromBody] ProductTypeCreateDTO productTypeCreateDTO)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _productTypeService.CreateProductTypeAsync(productTypeCreateDTO, tokenUserId.Value);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{productTypeId}")]
        public async Task<IActionResult> UpdateProductTypeAsync(short productTypeId, [FromBody] ProductTypeUpdateDTO productTypeUpdateDTO)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _productTypeService.UpdateProductTypeAsync(productTypeId, productTypeUpdateDTO, tokenUserId.Value);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{productTypeId}")]
        public async Task<IActionResult> DeleteProductTypeAsync(short productTypeId)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _productTypeService.DeleteProductTypeAsync(productTypeId, tokenUserId.Value);
            return result.ToActionResult();
        }

        [HttpGet("types-dropdown")]
        public async Task<IActionResult> GetProductTypesForDropdownAsync()
        {
            var result = await _productTypeService.GetProductTypesForDropdownAsync();
            return result.ToActionResult();
        }
    }
}
