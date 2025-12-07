using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Services;

namespace HairStudio.Services.Interfaces
{
    public interface IServiceService
    {
        Task<Result> CreateServiceAsync(ServiceCreateDTO serviceDTO, short tokenUserId);
        Task<Result<IEnumerable<ServiceDropdownDTO>>> GetServicesForDropdownAsync(short tokenUserId);
        Task<Result<object>> GetAllServicesAsync(int page, int rowsPerPage);
        Task<Result<ServicesByGenderDTO>> GetServicesByGenderAsync();
        Task<Result> UpdateServiceAsync(short serviceId, ServiceUpdateDTO serviceUpdateDTO, short tokenUserId);
        Task<Result> DeleteServiceAsync(short serviceId, short tokenUserId);
    }
}
