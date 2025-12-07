using HairStudio.Model.Models;
using HairStudio.Repository.Extensions;
using HairStudio.Repository.Interfaces;
using HairStudio.Services.Audit;
using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Brands;
using HairStudio.Services.Errors;
using HairStudio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairStudio.Services.Implementations
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public BrandService(IBrandRepository brandRepository, IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _brandRepository = brandRepository;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public async Task<Result<object>> GetPagedBrandsAsync(int page, int rowsPerPage)
        {
            if (page < 1 || rowsPerPage < 1)
                return Result<object>.Failure(ValidationErrors.NumberOfPages);

            var brandsQuery = _brandRepository.GetAll().Active(b => b.IsActive);
            var totalCount = await brandsQuery.CountAsync();

            var brandsForTable = await brandsQuery.Paged(page, rowsPerPage).Select(brand => new
            {
                brand.BrandId,
                brand.Name
            }).ToListAsync();

            return Result<object>.Success(new
            {
                TotalCount = totalCount,
                Brands = brandsForTable
            });
        }

        [Auditable("CREATE_BRAND")]
        public async Task<Result> CreateBrandAsync(BrandCreateDTO brandCreateDTO, short tokenUserId)
        {
            var user = await _userRepository.GetByIdAsync(tokenUserId);
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            if(await _brandRepository.BrandExistsByNameAsync(brandCreateDTO.Name))
                return Result.Failure(BrandErrors.BrandExists);

            Brand brand = new Brand
            {
                Name = brandCreateDTO.Name,
                IsActive = true
            };

            string message = $"User {user.FirstName} {user.LastName} created a brand {brandCreateDTO.Name}.";

            await _brandRepository.ExecuteInTransactionAsync(() =>
            {
                _brandRepository.Add(brand);
                _messageRepository.SaveMessage(user.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }


        [Auditable("UPDATE_BRAND")]
        public async Task<Result> UpdateBrandAsync(short brandId, BrandUpdateDTO brandUpdateDTO, short tokenUserId)
        {
            var user = await _userRepository.GetByIdAsync(tokenUserId);
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var existingBrand = await _brandRepository.GetByIdAsync(brandId);
            if (existingBrand == null || !existingBrand.IsActive)
                return Result.Failure(BrandErrors.BrandNotFound);

            existingBrand.Name = brandUpdateDTO.Name;
            string message = $"User {user.FirstName} {user.LastName} updated a brand {brandUpdateDTO.Name}.";

            await _brandRepository.ExecuteInTransactionAsync(() =>
            {
                _brandRepository.Update(existingBrand);
                _messageRepository.SaveMessage(user.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }


        [Auditable("DELETE_BRAND")]
        public async Task<Result> DeleteBrandAsync(short brandId, short tokenUserId)
        {
            var user = await _userRepository.GetByIdAsync(tokenUserId);
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var (brand, hasActiveProducts) = await _brandRepository.GetBrandWithCheckAsync(brandId);
            if (brand == null)
                return Result.Failure(BrandErrors.BrandNotFound);

            if (hasActiveProducts)
                return Result.Failure(BrandErrors.BrandHasProduct);

            brand.IsActive = false;
            string message = $"User {user.FirstName} {user.LastName} deleted a brand {brand.Name}.";

            await _brandRepository.ExecuteInTransactionAsync(() =>
            {
                _brandRepository.Update(brand);
                _messageRepository.SaveMessage(user.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        public async Task<Result<IEnumerable<BrandDTO>>> GetBrandsForDropdownAsync()
        {
            var brandsQuery = _brandRepository.GetAll().Active(b => b.IsActive);

            var brandsList = await brandsQuery
                .Select(o => new BrandDTO
                {
                    BrandId = o.BrandId,
                    Name = o.Name
                })
                .ToListAsync();

            return Result<IEnumerable<BrandDTO>>.Success(brandsList);
        }
    }
}
