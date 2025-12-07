using HairStudio.API.Infrastructure;
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
        private readonly ICurrentUserContext _currentUserContext;

        public ReservationController(IReservationService reservationService, ICurrentUserContext currentUserContext)
        {
            _reservationService = reservationService;
            _currentUserContext = currentUserContext;
        }

        [Authorize(Roles = "User,Employee,Administrator")]
        [HttpPost("create-reservation")]
        public async Task<IActionResult> CreateUserReservationAsync([FromBody] UserReservationCreateDTO clientReservationCreateDTO)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _reservationService.CreateUserReservationAsync(tokenUserId.Value, clientReservationCreateDTO);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Employee,Administrator")]
        [HttpPost("create-employee-reservation")]
        public async Task<IActionResult> CreateEmployeeReservationAsync([FromBody] EmployeeReservationCreateDTO employeeReservationCreateDTO)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _reservationService.CreateEmployeeReservationAsync(tokenUserId.Value, employeeReservationCreateDTO);
            return result.ToActionResult();
        }

        [HttpGet("reservations")]
        public async Task<IActionResult> GetEmployeeReservationsAsync(int employeeId, DateTime dateFrom, DateTime dateTo)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _reservationService.GetEmployeeReservationsAsync(tokenUserId.Value, employeeId, dateFrom, dateTo);
            return result.ToActionResult();
        }

        [Authorize(Roles = "User,Employee,Administrator")]
        [HttpGet("reservation-details/{reservationId}")]
        public async Task<IActionResult> GetReservationDetailsAsync(short reservationId)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _reservationService.GetReservationDetailsAsync(tokenUserId.Value, reservationId);
            return result.ToActionResult();
        }

        [Authorize(Roles = "User,Employee,Administrator")]
        [HttpPut("{reservationId}")]
        public async Task<IActionResult> CancelReservationAsync(short reservationId)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _reservationService.CancelReservationAsync(tokenUserId.Value, reservationId);
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
