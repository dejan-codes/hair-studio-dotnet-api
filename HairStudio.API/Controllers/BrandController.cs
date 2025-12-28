using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Brands;
using HairStudio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedBrandsAsync(int page, int rowsPerPage)
        {
            var result = await _brandService.GetPagedBrandsAsync(page, rowsPerPage);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateBrandAsync([FromBody] BrandCreateDTO brandCreateDTO)
        {
            var result = await _brandService.CreateBrandAsync(brandCreateDTO);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{brandId}")]
        public async Task<IActionResult> UpdateBrandAsync(short brandId, [FromBody] BrandUpdateDTO brandUpdateDTO)
        {
            var result = await _brandService.UpdateBrandAsync(brandId, brandUpdateDTO);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{brandId}")]
        public async Task<IActionResult> DeleteBrandAsync(short brandId)
        {
            var result = await _brandService.DeleteBrandAsync(brandId);
            return result.ToActionResult();
        }

        [HttpGet("brands-dropdown")]
        public async Task<IActionResult> GetBrandsForDropdownAsync()
        {
            var result = await _brandService.GetBrandsForDropdownAsync();
            return result.ToActionResult();
        }
    }
}
