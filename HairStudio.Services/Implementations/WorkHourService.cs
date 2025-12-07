using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;
using HairStudio.Services.Audit;
using HairStudio.Services.Common;
using HairStudio.Services.DTOs.WorkHours;
using HairStudio.Services.Errors;
using HairStudio.Services.Interfaces;

namespace HairStudio.Services.Implementations
{
    public class WorkHourService : IWorkHourService
    {
        private readonly IWorkHourRepository _workHourRepository;
        private readonly IUserRepository _userRepository;

        public WorkHourService(IWorkHourRepository workHourRepository, IUserRepository userRepository)
        {
            _workHourRepository = workHourRepository;
            _userRepository = userRepository;
        }

        [Auditable("CREATE_WORK_HOURS")]
        public async Task<Result> CreateWorkHoursAsync(List<WorkHourDTO> workHourDTOList)
        {
            var employeeIds = workHourDTOList
            .Select(o => o.EmployeeId)
            .Distinct()
            .ToList();

            var users = await _userRepository.GetActiveUsersByIdsAsync(employeeIds);

            var activeUsers = users
                .ToDictionary(u => u.UserId, u => u);

            var workHours = workHourDTOList
            .Where(dto => activeUsers.ContainsKey(dto.EmployeeId))
            .Select(o => new WorkHour
            {
                UserId = o.EmployeeId,
                Date = o.Date,
                TimeFrom = TimeSpan.Parse(o.TimeFrom),
                TimeTo = TimeSpan.Parse(o.TimeTo)
            }).ToList();

            await _workHourRepository.AddWorkHoursAsync(workHours);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<WorkHourForMonthDTO>>> GetWorkHoursAsync(DateTime date)
        {
            var workHours = await _workHourRepository.GetWorkHoursByMonthAsync(DateOnly.FromDateTime(date));

            return Result<IEnumerable<WorkHourForMonthDTO>>.Success(workHours.Select(o => new WorkHourForMonthDTO
            {
                WorkHourId = o.WorkHourId,
                EmployeeId = o.UserId,
                Day = o.Date.Day,
                TimeFrom = o.TimeFrom,
                TimeTo = o.TimeTo
            }));
        }

        public async Task<Result<IEnumerable<EmployeeWorkHoursDTO>>> GetEmployeeWorkHoursAsync(short employeeId, DateTime dateFrom, DateTime dateTo)
        {
            var workHours = await _workHourRepository.GetWorkHoursForEmployeeAsync(employeeId, dateFrom, dateTo);

            return Result<IEnumerable<EmployeeWorkHoursDTO>>.Success(workHours.Select(o => new EmployeeWorkHoursDTO
            {
                Date = o.Date,
                TimeFrom = o.TimeFrom,
                TimeTo = o.TimeTo
            }));
        }

        [Auditable("DELETE_WORK_HOURS")]
        public async Task<Result> DeleteWorkHourAsync(List<WorkHourDeleteDTO> workHourDeleteDTOList)
        {
            var pairs = workHourDeleteDTOList
            .Select(dto => (dto.EmployeeId, dto.Date))
            .ToList();

            await _workHourRepository.DeleteWorkHoursAsync(pairs);

            return Result.Success();
        }
    }
}
