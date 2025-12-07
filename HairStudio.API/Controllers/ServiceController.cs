using HairStudio.API.Infrastructure;
using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Services;
using HairStudio.Services.Errors;
using HairStudio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly ICurrentUserContext _currentUserContext;

        public ServiceController(IServiceService serviceService, ICurrentUserContext currentUserContext)
        {
            _serviceService = serviceService;
            _currentUserContext = currentUserContext;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateServiceAsync([FromForm] ServiceCreateDTO serviceCreateDTO)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _serviceService.CreateServiceAsync(serviceCreateDTO, tokenUserId.Value);
            return result.ToActionResult();
        }

        [HttpGet("service-dropdown")]
        public async Task<IActionResult> GetServicesForDropdownAsync()
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _serviceService.GetServicesForDropdownAsync(tokenUserId.Value);
            return result.ToActionResult();
        }

        [HttpGet("all-services")]
        public async Task<IActionResult> GetAllServicesAsync(int page, int rowsPerPage)
        {
            var result = await _serviceService.GetAllServicesAsync(page, rowsPerPage);
            return result.ToActionResult();
        }

        [HttpGet("service-list")]
        public async Task<IActionResult> GetServicesByGenderAsync()
        {
            var result = await _serviceService.GetServicesByGenderAsync();
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{serviceId}")]
        public async Task<IActionResult> UpdateServiceAsync(short serviceId, [FromForm] ServiceUpdateDTO serviceUpdateDTO)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _serviceService.UpdateServiceAsync(serviceId, serviceUpdateDTO, tokenUserId.Value);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{serviceId}")]
        public async Task<IActionResult> DeleteServiceAsync(short serviceId)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _serviceService.DeleteServiceAsync(serviceId, tokenUserId.Value);
            return result.ToActionResult();
        }
    }
}
