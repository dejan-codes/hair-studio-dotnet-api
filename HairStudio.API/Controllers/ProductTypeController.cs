using HairStudio.Services.Common;
using HairStudio.Services.DTOs.ProductTypes;
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

        public ProductTypeController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
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
            var result = await _productTypeService.CreateProductTypeAsync(productTypeCreateDTO);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{productTypeId}")]
        public async Task<IActionResult> UpdateProductTypeAsync(short productTypeId, [FromBody] ProductTypeUpdateDTO productTypeUpdateDTO)
        {
            var result = await _productTypeService.UpdateProductTypeAsync(productTypeId, productTypeUpdateDTO);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{productTypeId}")]
        public async Task<IActionResult> DeleteProductTypeAsync(short productTypeId)
        {
            var result = await _productTypeService.DeleteProductTypeAsync(productTypeId);
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
