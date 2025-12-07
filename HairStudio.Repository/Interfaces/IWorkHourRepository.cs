using HairStudio.Model.Models;

namespace HairStudio.Repository.Interfaces
{
    public interface IWorkHourRepository : IRepositoryBase<WorkHour>
    {
        Task AddWorkHoursAsync(IEnumerable<WorkHour> workingHours);
        Task<IEnumerable<WorkHour>> GetWorkHoursByMonthAsync(DateOnly date);
        Task<IEnumerable<WorkHour>> GetWorkHoursForEmployeeAsync(short employeeId, DateTime dateFrom, DateTime dateTo);
        Task<WorkHour?> GetWorkHourAsync(short employeeId, DateTime date);
        Task DeleteWorkHoursAsync(List<(short EmployeeId, DateTime Date)> pairs);
    }
}
