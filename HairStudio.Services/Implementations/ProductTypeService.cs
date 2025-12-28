using HairStudio.Model.Models;
using HairStudio.Repository.Extensions;
using HairStudio.Repository.Interfaces;
using HairStudio.Services.Audit;
using HairStudio.Services.Common;
using HairStudio.Services.DTOs.ProductTypes;
using HairStudio.Services.Errors;
using HairStudio.Services.Infrastructure;
using HairStudio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairStudio.Services.Implementations
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public ProductTypeService(ICurrentUserContext currentUserContext, IProductTypeRepository productTypeRepository, IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _currentUserContext = currentUserContext;
            _productTypeRepository = productTypeRepository;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public async Task<Result<object>> GetPagedProductTypesAsync(int page, int rowsPerPage)
        {
            if (page < 1 || rowsPerPage < 1)
                return Result<object>.Failure(ValidationErrors.NumberOfPages);

            var productTypesQuery = _productTypeRepository.GetAll().Active(b => b.IsActive);
            var totalCount = await productTypesQuery.CountAsync();

            var productTypesForTable = await productTypesQuery.Paged(page, rowsPerPage).Select(productType => new
            {
                productType.ProductTypeId,
                productType.Name
            }).ToListAsync();

            return Result<object>.Success(new
            {
                TotalCount = totalCount,
                ProductTypes = productTypesForTable
            });
        }

        [Auditable("CREATE_PRODUCT_TYPE")]
        public async Task<Result> CreateProductTypeAsync(ProductTypeCreateDTO productTypeCreateDTO)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserContext.GetAuthenticatedUserId());
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            if (await _productTypeRepository.ProductTypeExistsByNameAsync(productTypeCreateDTO.Name))
                return Result.Failure(ProductTypeErrors.ProductTypeExists);

            ProductType productType = new ProductType
            {
                Name = productTypeCreateDTO.Name,
                IsActive = true
            };

            string message = $"User {user.FirstName} {user.LastName} created a product type {productTypeCreateDTO.Name}.";

            await _productTypeRepository.ExecuteInTransactionAsync(() =>
            {
                _productTypeRepository.Add(productType);
                _messageRepository.SaveMessage(user.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        [Auditable("UPDATE_PRODUCT_TYPE")]
        public async Task<Result> UpdateProductTypeAsync(short productTypeId, ProductTypeUpdateDTO productTypeUpdateDTO)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserContext.GetAuthenticatedUserId());
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var existingProductType = await _productTypeRepository.GetByIdAsync(productTypeId);
            if (existingProductType == null || !existingProductType.IsActive)
                return Result.Failure(ProductTypeErrors.ProductTypeNotFound);

            existingProductType.Name = productTypeUpdateDTO.Name;
            string message = $"User {user.FirstName} {user.LastName} updated a product type {productTypeUpdateDTO.Name}.";

            await _productTypeRepository.ExecuteInTransactionAsync(() =>
            {
                _productTypeRepository.Update(existingProductType);
                _messageRepository.SaveMessage(user.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        [Auditable("DELETE_PRODUCT_TYPE")]
        public async Task<Result> DeleteProductTypeAsync(short productTypeId)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserContext.GetAuthenticatedUserId());
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var (productType, hasActiveProducts) = await _productTypeRepository.GetProductTypeWithCheckAsync(productTypeId);
            if (productType == null)
                return Result.Failure(ProductTypeErrors.ProductTypeNotFound);

            if (hasActiveProducts)
                return Result.Failure(ProductTypeErrors.ProductTypeHasProduct);

            productType.IsActive = false;
            string message = $"User {user.FirstName} {user.LastName} deleted a product type {productType.Name}.";

            await _productTypeRepository.ExecuteInTransactionAsync(() =>
            {
                _productTypeRepository.Update(productType);
                _messageRepository.SaveMessage(user.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        public async Task<Result<IEnumerable<ProductTypeDTO>>> GetProductTypesForDropdownAsync()
        {
            var productTypesQuery = _productTypeRepository.GetAll().Active(b => b.IsActive);

            var productTypesList = await productTypesQuery
                .Select(o => new ProductTypeDTO
                {
                    ProductTypeId = o.ProductTypeId,
                    Name = o.Name
                })
                .ToListAsync();

            return Result<IEnumerable<ProductTypeDTO>>.Success(productTypesList);
        }
    }
}
