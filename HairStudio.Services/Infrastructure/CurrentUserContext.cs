using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HairStudio.Services.Infrastructure
{
    public interface ICurrentUserContext
    {
        short GetAuthenticatedUserId();
        short? UserId { get; }
    }

    public class CurrentUserContext : ICurrentUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public short? UserId
        {
            get
            {
                var principal = _httpContextAccessor.HttpContext?.User;
                if (principal == null) return null;

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (short.TryParse(userIdClaim, out var userId))
                    return userId;

                return null;
            }
        }

        public short GetAuthenticatedUserId()
        {
            var userId = UserId;
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User context is required but was not found.");

            return userId.Value;
        }
    }
}
