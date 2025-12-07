using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairStudio.Repository.Implementations
{
    public class EmailConfirmationRepository : BaseRepository<EmailConfirmation>, IEmailConfirmationRepository
    {
        public EmailConfirmationRepository(HairStudioContext context) : base(context)
        {
        }

        public async Task<EmailConfirmation?> GetByCodeAsync(string code)
        {
            return await GetAll()
                .Include(ec => ec.User)
                .SingleOrDefaultAsync(ec => ec.ConfirmationCode == code);
        }

        public async Task<EmailConfirmation?> GetByUserIdAsync(short userId)
        {
            return await GetAll().FirstOrDefaultAsync(e => e.UserId == userId);
        }
    }
}
