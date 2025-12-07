using HairStudio.Model.Models;

namespace HairStudio.Repository.Interfaces
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserWithRolesAsync(string email);
        Task<User?> GetUnconfirmedUserByEmailAsync(string email);
        Task<User?> GetUserWithRolesAsync(short userId);
        Task<List<User>> GetEmployeesAsync();
        Task<List<User>> GetEmployeesWithReservationsAsync(DateTime from, DateTime to);
        Task<List<User>> GetActiveUsersByIdsAsync(IEnumerable<short> userIds);
    }
}
