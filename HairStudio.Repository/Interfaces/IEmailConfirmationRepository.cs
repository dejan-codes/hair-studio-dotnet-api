using HairStudio.Model.Models;

namespace HairStudio.Repository.Interfaces
{
    public interface IEmailConfirmationRepository : IRepositoryBase<EmailConfirmation>
    {
        Task<EmailConfirmation?> GetByCodeAsync(string code);
        Task<EmailConfirmation?> GetByUserIdAsync(short userId);
    }
}
