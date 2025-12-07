using HairStudio.Model.Models;

namespace HairStudio.Repository.Interfaces
{
    public interface IReservationRepository : IRepositoryBase<Reservation>
    {
        Task<IEnumerable<Reservation>> GetReservationsByEmployeeAndDateAsync(int employeeId, DateTime dateFrom, DateTime dateTo);
        Task<Reservation?> GetReservationWithDetails(short reservationId);
    }
}
