using HairStudio.Model.Models;
using HairStudio.Repository.Extensions;
using HairStudio.Repository.Interfaces;
using HairStudio.Services.Audit;
using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Services;
using HairStudio.Services.Enums;
using HairStudio.Services.Errors;
using HairStudio.Services.Infrastructure;
using HairStudio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairStudio.Services.Implementations
{
    public class ServiceService : IServiceService
    {
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public ServiceService(ICurrentUserContext currentUserContext, IServiceRepository serviceRepository, IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _currentUserContext = currentUserContext;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        [Auditable("CREATE_SERVICE")]
        public async Task<Result> CreateServiceAsync(ServiceCreateDTO serviceCreateDTO)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserContext.GetAuthenticatedUserId());
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            Service service = new Service
            {
                Description = serviceCreateDTO.Description,
                Price = serviceCreateDTO.Price,
                Discount = serviceCreateDTO.Discount,
                DurationMinutes = serviceCreateDTO.DurationMinutes,
                Name = serviceCreateDTO.Name,
                GenderId = serviceCreateDTO.GenderId,
                Image = FileHelper.FileToByteArray(serviceCreateDTO.Image),
                SequenceNumber = serviceCreateDTO.SequenceNumber,
                IsActive = true
            };

            string message = $"User {user.FirstName} {user.LastName} created a service {serviceCreateDTO.Name}.";

            await _serviceRepository.ExecuteInTransactionAsync(() =>
            {
                _serviceRepository.Add(service);
                _messageRepository.SaveMessage(user.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        public async Task<Result<IEnumerable<ServiceDropdownDTO>>> GetServicesForDropdownAsync()
        {
            var user = await _userRepository.GetUserWithRolesAsync(_currentUserContext.GetAuthenticatedUserId());
            if (user == null || !user.IsActive)
                return Result<IEnumerable<ServiceDropdownDTO>>.Failure(UserErrors.UserNotFound);

            var services = _serviceRepository.GetAll().Where(o => o.IsActive);

            bool isAdminOrEmployee = user.Roles.Any(o => o.RoleId == (int)RoleEnum.Administrator || o.RoleId == (int)RoleEnum.Employee);

            if (user == null || !isAdminOrEmployee)
                services = services.Where(o => !o.Name.Equals("Disable"));

            var serviceDropdownDTO = await services.OrderBy(o => o.SequenceNumber)
                .Select(o => new ServiceDropdownDTO
                {
                    ServiceId = o.ServiceId,
                    Name = o.Name,
                    DurationMinutes = o.DurationMinutes,
                }).ToListAsync();

            return Result<IEnumerable<ServiceDropdownDTO>>.Success(serviceDropdownDTO);
        }

        public async Task<Result<object>> GetAllServicesAsync(int page, int rowsPerPage)
        {
            if (page < 1 || rowsPerPage < 1)
                return Result<object>.Failure(ValidationErrors.NumberOfPages);

            var servicesQuery = _serviceRepository.GetAll().Active(s => s.IsActive)
                .Where(o => !o.Name.Equals("Disable"))
                .OrderBy(o => o.Gender).ThenBy(o => o.SequenceNumber);

            var totalCount = await servicesQuery.CountAsync();

            var servicesForTable = await servicesQuery
                .Paged(page, rowsPerPage)
                .Select(o => new ServiceDTO
                {
                    ServiceId = o.ServiceId,
                    Name = o.Name,
                    DurationMinutes = o.DurationMinutes,
                    Description = o.Description,
                    Discount = o.Discount,
                    GenderId = o.Gender.GenderId,
                    Image = o.Image,
                    SequenceNumber = o.SequenceNumber,
                    Price = o.Price
                }).ToListAsync();

            return Result<object>.Success(new
            {
                TotalCount = totalCount,
                Services = servicesForTable
            });
        }

        public async Task<Result<ServicesByGenderDTO>> GetServicesByGenderAsync()
        {
            var servicesList = await _serviceRepository.GetAll().Active(s => s.IsActive)
                .Where(o => !o.Name.Equals("Disable"))
                .OrderBy(o => o.Gender)
                .ThenBy(o => o.SequenceNumber)
                .Select(o => new ServiceSummaryDTO
                {
                    ServiceId = o.ServiceId,
                    Name = o.Name,
                    Description = o.Description,
                    Price = o.Price,
                    Gender = o.Gender.Name,
                    Image = o.Image,
                })
                .ToListAsync();

            var menServices = servicesList.Where(s => s.Gender == "Male").ToList();
            var womenServices = servicesList.Where(s => s.Gender == "Female").ToList();

            return Result<ServicesByGenderDTO>.Success(new ServicesByGenderDTO {
                MaleServices = menServices,
                FemaleServices = womenServices
            });
        }

        [Auditable("UPDATE_SERVICE")]
        public async Task<Result> UpdateServiceAsync(short serviceId, ServiceUpdateDTO serviceUpdateDTO)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserContext.GetAuthenticatedUserId());
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var existingService = await _serviceRepository.GetByIdAsync(serviceId);
            if (existingService == null || !existingService.IsActive || existingService.Name.Equals("Disable"))
                return Result.Failure(ServiceErrors.ServiceNotFound);

            existingService.Description = serviceUpdateDTO.Description;
            existingService.Price = serviceUpdateDTO.Price;
            existingService.Discount = serviceUpdateDTO.Discount;
            existingService.DurationMinutes = serviceUpdateDTO.DurationMinutes;
            existingService.Name = serviceUpdateDTO.Name;
            existingService.GenderId = serviceUpdateDTO.GenderId;
            existingService.SequenceNumber = serviceUpdateDTO.SequenceNumber;
            if (serviceUpdateDTO.Image != null)
                existingService.Image = FileHelper.FileToByteArray(serviceUpdateDTO.Image);

            string message = $"User {user.FirstName} {user.LastName} updated a service {serviceUpdateDTO.Name}.";

            await _serviceRepository.ExecuteInTransactionAsync(() =>
            {
                _serviceRepository.Update(existingService);
                _messageRepository.SaveMessage(user.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        [Auditable("DELETE_SERVICE")]
        public async Task<Result> DeleteServiceAsync(short serviceId)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserContext.GetAuthenticatedUserId());
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null || !service.IsActive || service.Name.Equals("Disable"))
                return Result.Failure(ServiceErrors.ServiceNotFound);

            service.IsActive = false;
            string message = $"User {user.FirstName} {user.LastName} deleted a service {service.Name}.";

            await _serviceRepository.ExecuteInTransactionAsync(() =>
            {
                _serviceRepository.Update(service);
                _messageRepository.SaveMessage(user.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }
    }
}
