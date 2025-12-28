using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Reservations;

namespace HairStudio.Services.Interfaces
{
    public interface IReservationService
    {
        Task<Result> CreateUserReservationAsync(UserReservationCreateDTO userReservationCreateDTO);
        Task<Result> CreateEmployeeReservationAsync(EmployeeReservationCreateDTO employeeReservationCreateDTO);
        Task<Result<IEnumerable<ReservationSummaryDTO>>> GetEmployeeReservationsAsync(int employeeId, DateTime dateFrom, DateTime dateTo);
        Task<Result<ReservationDetailsDTO?>> GetReservationDetailsAsync(short reservationId);
        Task<Result> CancelReservationAsync(short reservationId);
        Task<byte[]> ExportCalendarByEmployeeAsync(DateTime from, DateTime to);
    }
}
