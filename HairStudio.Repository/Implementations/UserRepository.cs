using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;
using HairStudio.Services.Enums;
using Microsoft.EntityFrameworkCore;

namespace HairStudio.Repository.Implementations
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(HairStudioContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email) => await GetAll().FirstOrDefaultAsync(u => u.Email.Equals(email));

        public async Task<User?> GetUserByEmailAsync(string email) => await GetAll().SingleOrDefaultAsync(u => u.Email == email && u.IsActive);

        public async Task<User?> GetUserWithRolesAsync(string email)
        {
            return await GetAll()
                .Include(u => u.Roles)
                .Include(u => u.PasswordResetTokens)
                .FirstOrDefaultAsync(u => u.Email.Equals(email) && u.IsActive);
        }
        
        public async Task<User?> GetUnconfirmedUserByEmailAsync(string email)
        {
            return await GetAll()
                .FirstOrDefaultAsync(u => u.Email == email && !u.EmailConfirmed);
        }

        public async Task<User?> GetUserWithRolesAsync(short userId)
        {
            return await GetAll()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<List<User>> GetEmployeesAsync()
        {
            return await GetAll()
                .Where(u => u.Roles.Any(r => r.RoleId == (int)RoleEnum.Employee) && u.IsActive)
                .ToListAsync();
        }

        public async Task<List<User>> GetEmployeesWithReservationsAsync(DateTime from, DateTime to)
        {
            return await GetAll()
            .Where(u => u.IsActive && u.Roles.Any(r => r.RoleId == (int)RoleEnum.Employee))
            .Select(u => new User
            {
                UserId = u.UserId,
                IsActive = u.IsActive,
                Bio = u.Bio,
                Email = u.Email,
                EmailConfirmed = u.EmailConfirmed,
                FirstName = u.FirstName,
                LastName = u.LastName,
                ReservationEmployees = u.ReservationEmployees
                    .Where(r => r.DateFrom.Date >= from.Date && r.DateFrom.Date <= to.Date)
                    .Select(r => new Reservation
                    {
                        ReservationId = r.ReservationId,
                        DateFrom = r.DateFrom,
                        DateTo = r.DateTo,
                        Service = r.Service,
                        ClientCustomer = r.ClientCustomer,
                        ClientUser = r.ClientUser,
                        Employee = r.Employee
                    }).ToList()
            })
            .ToListAsync();
        }

        public async Task<List<User>> GetActiveUsersByIdsAsync(IEnumerable<short> userIds)
        {
            return await _context.Users
                .Where(u => userIds.Contains(u.UserId) && u.IsActive)
                .ToListAsync();
        }
    }
}
