using System.Security.Claims;

namespace HairStudio.API.Infrastructure
{
    public interface ICurrentUserContext
    {
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
    }
}
