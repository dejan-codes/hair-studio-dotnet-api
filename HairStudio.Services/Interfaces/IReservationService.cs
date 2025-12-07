using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Reservations;

namespace HairStudio.Services.Interfaces
{
    public interface IReservationService
    {
        Task<Result> CreateUserReservationAsync(short tokenUserId, UserReservationCreateDTO userReservationCreateDTO);
        Task<Result> CreateEmployeeReservationAsync(short tokenUserId, EmployeeReservationCreateDTO employeeReservationCreateDTO);
        Task<Result<IEnumerable<ReservationSummaryDTO>>> GetEmployeeReservationsAsync(short tokenUserId, int employeeId, DateTime dateFrom, DateTime dateTo);
        Task<Result<ReservationDetailsDTO?>> GetReservationDetailsAsync(short tokenUserId, short reservationId);
        Task<Result> CancelReservationAsync(short tokenUserId, short reservationId);
        Task<byte[]> ExportCalendarByEmployeeAsync(DateTime from, DateTime to);
    }
}
