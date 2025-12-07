using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;

namespace HairStudio.Repository.Implementations
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(HairStudioContext context) : base(context)
        {
        }
    }
}
