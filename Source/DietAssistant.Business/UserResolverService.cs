using DietAssistant.Business.Contracts;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

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

        public async Task<User> GetCurrentUserAsync()
        {
            if (int.TryParse(_httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out int userId))
                return null; 

            var user = await _userRepository.GetByIdAsync(userId);

            return user;
        }
    }
}
