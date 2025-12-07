using HairStudio.Model.Models;

namespace HairStudio.Repository.Interfaces
{
    public interface IRoleRepository : IRepositoryBase<Role>
    {
        Task<List<Role>> GetRolesByIdsAsync(List<int> roleIds);
    }
}
