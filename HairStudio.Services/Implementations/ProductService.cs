using HairStudio.Model.Enums;
using HairStudio.Model.Models;
using HairStudio.Repository.Extensions;
using HairStudio.Repository.Interfaces;
using HairStudio.Services.Audit;
using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Orders;
using HairStudio.Services.DTOs.Products;
using HairStudio.Services.Enums;
using HairStudio.Services.Errors;
using HairStudio.Services.Infrastructure;
using HairStudio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stripe.Checkout;

namespace HairStudio.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IConfiguration _configuration;

        public ProductService(ICurrentUserContext currentUserContext, IProductRepository productRepository, IOrderRepository orderRepository, IBrandRepository brandRepository, IProductTypeRepository productTypeRepository, IUserRepository userRepository, IMessageRepository messageRepository, IConfiguration configuration)
        {
            _currentUserContext = currentUserContext;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _brandRepository = brandRepository;
            _productTypeRepository = productTypeRepository;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _configuration = configuration;
        }

        public async Task<Result<object>> GetProductsAsync(int page, int rowsPerPage, string? search, int? brand, int? type, int minPrice, int maxPrice, string? sortOrder = "asc")
        {
            if (page < 1 || rowsPerPage < 1)
                return Result<object>.Failure(ValidationErrors.NumberOfPages);

            var productsQuery = _productRepository.GetActiveProducts()
                .Where(o => o.Price > minPrice && o.Price < maxPrice);

            if (brand != null)
                productsQuery = productsQuery.Where(o => o.BrandId == brand);

            if (type != null)
                productsQuery = productsQuery.Where(o => o.ProductTypeId == type);

            if (!string.IsNullOrWhiteSpace(search))
            {
                productsQuery = productsQuery.Where(o =>
                    o.Name.Contains(search) ||
                    o.Description.Contains(search));
            }

            if (!string.IsNullOrEmpty(sortOrder))
            {
                productsQuery = sortOrder.Equals("asc")
                    ? productsQuery.OrderBy(o => o.Price)
                    : sortOrder.Equals("popularity")
                        ? productsQuery.OrderByDescending(o => o.NumberOfPurchases)
                        : productsQuery.OrderByDescending(o => o.Price);
            }
            else
            {
                productsQuery = productsQuery.OrderBy(o => o.SequenceNumber);
            }

            var totalCount = await productsQuery.CountAsync();

            var productsForTable = await productsQuery.Paged(page, rowsPerPage).Select(o => new ProductDisplayDTO
            {
                ProductId = o.ProductId,
                Description = o.Description,
                Image = o.Image,
                Name = o.Name,
                Price = o.Price,
                Stock = o.Stock,
                BrandId = o.BrandId,
                ProductTypeId = o.ProductTypeId,
                Brand = o.Brand.Name,
                ProductType = o.ProductType.Name,
                SequenceNumber = o.SequenceNumber,
            })
            .ToListAsync();

            return Result<object>.Success(new { 
                TotalCount = totalCount,
                Products = productsForTable
            });
        }

        public async Task<Result<object>> GetMostPopularProductsAsync()
        {
            var products = _productRepository.GetActiveProducts()
                .OrderByDescending(o => o.NumberOfPurchases)
                .Take(5);

            var productsDTO = await products
                .Select(o => new ProductDisplayDTO
                {
                    ProductId = o.ProductId,
                    Image = o.Image,
                    Name = o.Name,
                    Price = o.Price,
                    Stock = o.Stock,
                    Brand = o.Brand.Name,
                    ProductType = o.ProductType.Name,
                    Description = o.Description
                })
                .ToListAsync();

            return Result<object>.Success(productsDTO);
        }

        public async Task<Result<object>> GetOrdersAsync(int page, int rowsPerPage)
        {
            var tokenUserId = _currentUserContext.GetAuthenticatedUserId();
            if (page < 1 || rowsPerPage < 1)
                return Result<object>.Failure(ValidationErrors.NumberOfPages);

            var tokenUser = await _userRepository.GetByIdAsync(tokenUserId);
            if (tokenUser == null || !tokenUser.IsActive)
                return Result<object>.Failure(UserErrors.UserNotFound);

            bool isAdminOrEmployee = tokenUser.Roles.Any(r => r.RoleId == (int)RoleEnum.Administrator || r.RoleId == (int)RoleEnum.Employee);

            var ordersQuery = _orderRepository.GetOrders();
            if (!isAdminOrEmployee)
                ordersQuery = ordersQuery.Where(o => o.UserId == tokenUserId);

            var totalCount = await ordersQuery.CountAsync();

            var ordersForTable = await ordersQuery
                .OrderBy(o => o.OrderStatus.OrderStatusId)
                .ThenByDescending(o => o.OrderId)
                .Paged(page, rowsPerPage)
                .Select(o => new OrderSummaryDTO
                {
                    OrderId = o.OrderId,
                    FullName = o.User.FirstName + " " + o.User.LastName,
                    OrderStatusId = o.OrderStatus.OrderStatusId,
                    PaymentStatus = o.PaymentStatus.Name,
                    PaidAt = o.PaidAt,
                    OrdersDTO = o.OrderItems.Select(oi => new OrderItemDetailDTO
                    {
                        Price = oi.Price,
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return Result<object>.Success(new
            {
                TotalCount = totalCount,
                Orders = ordersForTable
            });
        }

        [Auditable("CREATE_PRODUCT")]
        public async Task<Result> CreateProductAsync(ProductCreateDTO productCreateDTO)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserContext.GetAuthenticatedUserId());
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var brand = await _brandRepository.GetByIdAsync(productCreateDTO.BrandId);
            if (brand == null || !brand.IsActive)
                return Result<string>.Failure(BrandErrors.BrandNotFound);

            var productType = await _productTypeRepository.GetByIdAsync(productCreateDTO.ProductTypeId);
            if (productType == null || !productType.IsActive)
                return Result<string>.Failure(ProductTypeErrors.ProductTypeNotFound);

            Product product = new Product
            {
                Description = productCreateDTO.Description,
                Image = FileHelper.FileToByteArray(productCreateDTO.Image),
                Name = productCreateDTO.Name,
                Price = productCreateDTO.Price,
                Stock = productCreateDTO.Stock,
                BrandId = productCreateDTO.BrandId,
                ProductTypeId = productCreateDTO.ProductTypeId,
                SequenceNumber = productCreateDTO.SequenceNumber,
                NumberOfPurchases = 0,
                IsActive = true
            };

            await _productRepository.ExecuteInTransactionAsync(() =>
            {
                _productRepository.Add(product);
                _messageRepository.SaveMessage(
                    user.UserId,
                    $"User {user.FirstName} {user.LastName} created a product {productCreateDTO.Name}."
                );
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        [Auditable("BUY_PRODUCTS")]
        public async Task<Result<UrlResponseDTO>> BuyProductsAsync(List<BuyProductDTO> buyProductDTOList)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserContext.GetAuthenticatedUserId());
            if (user == null || !user.IsActive)
                return Result<UrlResponseDTO>.Failure(UserErrors.UserNotFound);

            var order = new Order
            {
                UserId = user.UserId,
                OrderStatusId = (int) OrderStatusEnum.Pending,
                CreatedAt = DateTime.Now,
                PaymentStatusId = (int) PaymentStatusEnum.Unpaid,
                OrderItems = new List<OrderItem>()
            };

            decimal totalPrice = 0;
            var lineItems = new List<SessionLineItemOptions>();

            Stripe.StripeConfiguration.ApiKey = _configuration["StripeApiKey"];

            foreach (var productDTO in buyProductDTOList)
            {
                Product? product = await _productRepository.GetByIdAsync(productDTO.ProductId);
                if (product == null)
                    return Result<UrlResponseDTO>.Failure(ProductErrors.ProductNotFound);

                product.Stock -= productDTO.Quantity;
                product.NumberOfPurchases += productDTO.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = productDTO.ProductId,
                    Quantity = productDTO.Quantity,
                    Price = productDTO.Price
                };

                order.OrderItems.Add(orderItem);
                totalPrice += productDTO.Price * productDTO.Quantity;

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(product.Price * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = product.Name
                        }
                    },
                    Quantity = productDTO.Quantity
                });
            }

            order.TotalPrice = totalPrice;

            await _orderRepository.AddAndSaveAsync(order);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{_configuration["FrontendUrl"]}/orders?status=success",
                CancelUrl = $"{_configuration["FrontendUrl"]}/orders?status=cancel",
                Metadata = new Dictionary<string, string>
            {
                { "userId", user.UserId.ToString() },
                { "orderId", order.OrderId.ToString() },
                { "cartJson", JsonConvert.SerializeObject(buyProductDTOList) }
            }
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            UrlResponseDTO urlResponse = new UrlResponseDTO { Url = session.Url };
            return Result<UrlResponseDTO>.Success(urlResponse);
        }

        public object GetSessionDetails(string sessionId)
        {
            var service = new SessionService();
            var session = service.Get(sessionId);
            return new
            {
                session.Id,
                session.CustomerEmail,
                session.AmountTotal,
                session.PaymentStatus,
                session.Metadata
            };
        }

        [Auditable("HANDLE_STRIPE_WEBHOOK")]
        public async Task<Result> HandleStripeWebhookAsync(string json, string stripeSignature)
        {
            var secret = _configuration["StripeWebhookSecret"];
            var stripeEvent = Stripe.EventUtility.ConstructEvent(
                json,
                stripeSignature,
                secret
            );

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;
                if (session == null)
                    return Result.Failure(ValidationErrors.InvalidData);
                var orderIdStr = session.Metadata["orderId"];

                if (int.TryParse(orderIdStr, out int orderId))
                {
                    var order = await _orderRepository.GetByIdAsync(orderId);
                    if (order != null && order.PaymentStatus.Name != PaymentStatusEnum.Paid.ToString())
                    {
                        order.PaymentStatusId = (int) PaymentStatusEnum.Paid;
                        order.PaidAt = DateTime.Now;
                        await _orderRepository.UpdateAndSaveAsync(order);
                    }
                }
            }

            return Result.Success();
        }

        [Auditable("UPDATE_PRODUCT")]
        public async Task<Result> UpdateProductAsync(short productId, ProductUpdateDTO productUpdateDTO)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserContext.GetAuthenticatedUserId());
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var brand = await _brandRepository.GetByIdAsync(productUpdateDTO.BrandId);
            if (brand == null || !brand.IsActive)
                return Result<string>.Failure(BrandErrors.BrandNotFound);

            var productType = await _productTypeRepository.GetByIdAsync(productUpdateDTO.ProductTypeId);
            if (productType == null || !productType.IsActive)
                return Result<string>.Failure(ProductTypeErrors.ProductTypeNotFound);

            var existingProduct = await _productRepository.GetByIdAsync(productId);
            if (existingProduct == null || !existingProduct.IsActive)
                return Result<string>.Failure(ProductErrors.ProductNotFound);

            existingProduct.Name = productUpdateDTO.Name;
            existingProduct.Description = productUpdateDTO.Description;
            existingProduct.Price = productUpdateDTO.Price;
            existingProduct.Stock = productUpdateDTO.Stock;
            existingProduct.BrandId = productUpdateDTO.BrandId;
            existingProduct.ProductTypeId = productUpdateDTO.ProductTypeId;
            existingProduct.SequenceNumber = productUpdateDTO.SequenceNumber;

            if (productUpdateDTO.Image != null)
                existingProduct.Image = FileHelper.FileToByteArray(productUpdateDTO.Image);

            await _productRepository.ExecuteInTransactionAsync(() =>
            {
                _productRepository.Update(existingProduct);
                _messageRepository.SaveMessage(
                    user.UserId,
                    $"User {user.FirstName} {user.LastName} updated a product {productUpdateDTO.Name}."
                );
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        [Auditable("CHANGE_ORDER_STATUS")]
        public async Task<Result> ChangeOrderStatusAsync(int orderId, short orderStatusId)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserContext.GetAuthenticatedUserId());
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var existingOrder = await _orderRepository.GetByIdAsync(orderId);
            if (existingOrder == null)
                return Result.Failure(OrderErrors.OrderNotFound);

            var orderStatus = await _orderRepository.GetOrderStatusByIdAsync(orderStatusId);
            if (orderStatus == null)
                return Result.Failure(OrderErrors.OrderStatusNotFound);

            existingOrder.OrderStatus = orderStatus;

            var existingUser = await _userRepository.GetByIdAsync(existingOrder.UserId);
            if (existingUser == null || !existingUser.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            await _productRepository.ExecuteInTransactionAsync(() =>
            {
                _orderRepository.Update(existingOrder);
                _messageRepository.SaveMessage(
                    user.UserId,
                    $"User {user.FirstName} {user.LastName} changed {existingUser.FirstName} {existingUser.LastName}'s order to {orderStatus.Name}."
                );
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        [Auditable("DELETE_PRODUCT")]
        public async Task<Result> DeleteProductAsync(short productId)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserContext.GetAuthenticatedUserId());
            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || !product.IsActive)
                return Result.Failure(ProductErrors.ProductNotFound);

            product.IsActive = false;

            await _productRepository.ExecuteInTransactionAsync(() =>
            {
                _productRepository.Update(product);
                _messageRepository.SaveMessage(
                    user.UserId,
                    $"User {user.FirstName} {user.LastName} deleted a product {product.Name}."
                );
                return Task.CompletedTask;
            });

            return Result.Success();
        }
    }
}
