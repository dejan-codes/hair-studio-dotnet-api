using HairStudio.API.Infrastructure;
using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Brands;
using HairStudio.Services.Errors;
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
        private readonly ICurrentUserContext _currentUserContext;

        public BrandController(IBrandService brandService, ICurrentUserContext currentUserContext)
        {
            _brandService = brandService;
            _currentUserContext = currentUserContext;
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
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _brandService.CreateBrandAsync(brandCreateDTO, tokenUserId.Value);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{brandId}")]
        public async Task<IActionResult> UpdateBrandAsync(short brandId, [FromBody] BrandUpdateDTO brandUpdateDTO)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _brandService.UpdateBrandAsync(brandId, brandUpdateDTO, tokenUserId.Value);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{brandId}")]
        public async Task<IActionResult> DeleteBrandAsync(short brandId)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _brandService.DeleteBrandAsync(brandId, tokenUserId.Value);
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
