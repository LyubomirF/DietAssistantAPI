using DietAssistant.Business.Contracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DietAssistant.Business
{
    public class UserResolverService : IUserResolverService
    {
        private readonly HttpContext _httpContext;

        public UserResolverService(IHttpContextAccessor httpContextAccessor)
            => _httpContext = httpContextAccessor.HttpContext;

        public Int32? GetCurrentUserId()
        {
            var claim = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(claim, out int userId))
                return null;

            return userId;
        }
    }
}
