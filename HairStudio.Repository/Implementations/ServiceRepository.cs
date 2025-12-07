using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;

namespace HairStudio.Repository.Implementations
{
    public class ServiceRepository : BaseRepository<Service>, IServiceRepository
    {
        public ServiceRepository(HairStudioContext context) : base(context)
        {
        }
    }
}
