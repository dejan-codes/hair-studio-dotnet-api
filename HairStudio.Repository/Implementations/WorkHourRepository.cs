using Microsoft.EntityFrameworkCore;
using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;
using System.Linq.Expressions;

namespace HairStudio.Repository.Implementations
{
    public class WorkHourRepository : BaseRepository<WorkHour>, IWorkHourRepository
    {
        public WorkHourRepository(HairStudioContext context) : base(context)
        {
        }

        public async Task AddWorkHoursAsync(IEnumerable<WorkHour> workingHours)
        {
            await _context.WorkHours.AddRangeAsync(workingHours);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<WorkHour>> GetWorkHoursByMonthAsync(DateOnly date)
        {
            return await GetAll()
                .Where(o => o.Date.Year == date.Year && o.Date.Month == date.Month)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkHour>> GetWorkHoursForEmployeeAsync(short employeeId, DateTime dateFrom, DateTime dateTo)
        {
            return await GetAll()
                .Where(o => o.UserId == employeeId && o.Date >= dateFrom && o.Date <= dateTo)
                .ToListAsync();
        }

        public async Task<WorkHour?> GetWorkHourAsync(short employeeId, DateTime date)
        {
            return await GetAll().FirstOrDefaultAsync(o =>
                o.UserId == employeeId &&
                o.Date.Year == date.Year &&
                o.Date.Month == date.Month &&
                o.Date.Day == date.Day);
        }

        public async Task DeleteWorkHoursAsync(List<(short EmployeeId, DateTime Date)> pairs)
        {
            if (pairs == null || !pairs.Any())
            {
                return;
            }

            Expression<Func<WorkHour, bool>> predicate = null;

            foreach (var pair in pairs)
            {
                Expression<Func<WorkHour, bool>> pairPredicate = wh =>
                    wh.UserId == pair.EmployeeId && wh.Date == pair.Date;

                if (predicate == null)
                {
                    predicate = pairPredicate;
                }
                else
                {
                    var invokedExpr = Expression.Invoke(pairPredicate, predicate.Parameters.Cast<Expression>());
                    predicate = Expression.Lambda<Func<WorkHour, bool>>(
                        Expression.OrElse(predicate.Body, invokedExpr), predicate.Parameters);
                }
            }

            var workHoursToDelete = await _context.WorkHours
               .Where(predicate)
               .ToListAsync();

            if (workHoursToDelete.Any())
            {
                _context.WorkHours.RemoveRange(workHoursToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}
