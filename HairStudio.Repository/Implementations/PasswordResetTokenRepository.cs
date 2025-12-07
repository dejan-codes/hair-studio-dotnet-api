using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;

namespace HairStudio.Repository.Implementations
{
    public class PasswordResetTokenRepository : BaseRepository<PasswordResetToken>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(HairStudioContext context) : base(context)
        {
        }
    }
}
