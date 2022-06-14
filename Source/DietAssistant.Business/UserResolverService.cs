using DietAssistant.Business.Contracts;
using DietAssistant.DataAccess.Contracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DietAssistant.Business
{
    public class UserResolverService : IUserResolverService
    {
        private readonly HttpContext _httpContext;
        private readonly IUserRepository _userRepository;

        public UserResolverService(
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _userRepository = userRepository;
        }

        public Int32? GetCurrentUserId()
        {
            var claim = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(claim, out int userId))
                return null;

            return userId;
        }
    }
}
