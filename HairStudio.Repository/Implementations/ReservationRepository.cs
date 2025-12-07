using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairStudio.Repository.Implementations
{
    public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(HairStudioContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByEmployeeAndDateAsync(int employeeId, DateTime dateFrom, DateTime dateTo)
        {
            return await GetAll()
                .Include(r => r.ClientUser)
                .Include(r => r.Service)
                .Include(r => r.ClientCustomer)
                .Where(r => r.EmployeeId == employeeId
                            && r.IsActive
                            && dateFrom.Date <= r.DateFrom.Date
                            && dateTo.Date >= r.DateFrom.Date)
                .ToListAsync();
        }

        public async Task<Reservation?> GetReservationWithDetails(short reservationId)
        {
            return await GetAll()
                .Include(r => r.ClientUser)
                .Include(r => r.Service)
                .Include(r => r.ClientCustomer)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);
        }
    }
}
