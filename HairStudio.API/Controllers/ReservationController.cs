using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Reservations;
using HairStudio.Services.Errors;
using HairStudio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [Authorize(Roles = "User,Employee,Administrator")]
        [HttpPost("create-reservation")]
        public async Task<IActionResult> CreateUserReservationAsync([FromBody] UserReservationCreateDTO clientReservationCreateDTO)
        {
            var result = await _reservationService.CreateUserReservationAsync(clientReservationCreateDTO);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Employee,Administrator")]
        [HttpPost("create-employee-reservation")]
        public async Task<IActionResult> CreateEmployeeReservationAsync([FromBody] EmployeeReservationCreateDTO employeeReservationCreateDTO)
        {
            var result = await _reservationService.CreateEmployeeReservationAsync(employeeReservationCreateDTO);
            return result.ToActionResult();
        }

        [HttpGet("reservations")]
        public async Task<IActionResult> GetEmployeeReservationsAsync(int employeeId, DateTime dateFrom, DateTime dateTo)
        {
            var result = await _reservationService.GetEmployeeReservationsAsync(employeeId, dateFrom, dateTo);
            return result.ToActionResult();
        }

        [Authorize(Roles = "User,Employee,Administrator")]
        [HttpGet("reservation-details/{reservationId}")]
        public async Task<IActionResult> GetReservationDetailsAsync(short reservationId)
        {
            var result = await _reservationService.GetReservationDetailsAsync(reservationId);
            return result.ToActionResult();
        }

        [Authorize(Roles = "User,Employee,Administrator")]
        [HttpPut("{reservationId}")]
        public async Task<IActionResult> CancelReservationAsync(short reservationId)
        {
            var result = await _reservationService.CancelReservationAsync(reservationId);
            return result.ToActionResult();
        }

        [HttpGet("excel")]
        public async Task<IActionResult> ExportCalendarByEmployeeAsync([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if(from > to)
                return Result.Failure(ValidationErrors.InvalidData).ToActionResult();

            var fileContent = await _reservationService.ExportCalendarByEmployeeAsync(from, to);
            var fileName = $"Reservations_Report_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx";

            return File(fileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
    }
}
