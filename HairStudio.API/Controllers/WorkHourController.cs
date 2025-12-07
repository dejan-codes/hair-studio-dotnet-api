using HairStudio.Services.Common;
using HairStudio.Services.DTOs.WorkHours;
using HairStudio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkHourController : ControllerBase
    {
        private readonly IWorkHourService _workHourService;

        public WorkHourController(IWorkHourService workHourService)
        {
            _workHourService = workHourService;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateWorkHoursAsync([FromBody] List<WorkHourDTO> workingHourDTOList)
        {
            var result = await _workHourService.CreateWorkHoursAsync(workingHourDTOList);
            return result.ToActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkHoursAsync(DateTime date)
        {
            var result = await _workHourService.GetWorkHoursAsync(date);
            return result.ToActionResult();
        }

        [HttpGet("employee")]
        public async Task<IActionResult> GetEmployeeWorkHoursAsync(short employeeId, DateTime dateFrom, DateTime dateTo)
        {
            var result = await _workHourService.GetEmployeeWorkHoursAsync(employeeId, dateFrom, dateTo);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete]
        public async Task<IActionResult> DeleteWorkHourAsync([FromBody] List<WorkHourDeleteDTO> workingHourDeleteDTOList)
        {
            var result = await _workHourService.DeleteWorkHourAsync(workingHourDeleteDTOList);
            return result.ToActionResult();
        }
    }
}
