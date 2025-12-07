using HairStudio.Services.Common;
using HairStudio.Services.DTOs.WorkHours;

namespace HairStudio.Services.Interfaces
{
    public interface IWorkHourService
    {
        Task<Result> CreateWorkHoursAsync(List<WorkHourDTO> workHourDTOList);
        Task<Result<IEnumerable<WorkHourForMonthDTO>>> GetWorkHoursAsync(DateTime date);
        Task<Result<IEnumerable<EmployeeWorkHoursDTO>>> GetEmployeeWorkHoursAsync(short employeeId, DateTime dateFrom, DateTime dateTo);
        Task<Result> DeleteWorkHourAsync(List<WorkHourDeleteDTO> workHourDeleteDTOList);
    }
}
