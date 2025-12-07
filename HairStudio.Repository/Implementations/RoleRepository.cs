using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairStudio.Repository.Implementations
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(HairStudioContext context) : base(context)
        {
        }

        public async Task<List<Role>> GetRolesByIdsAsync(List<int> roleIds)
        {
            return await GetAll()
                         .Where(r => roleIds.Contains(r.RoleId))
                         .ToListAsync();
        }
    }
}
