using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.UserStats.Requests;

namespace DietAssistant.Business
{
    public class UserStatsService
    {
        private readonly IUserResolverService _userResolverService;

        public UserStatsService(
            IUserResolverService userResolverService)
        {
            _userResolverService = userResolverService;
        }

        public async Task SetUserStatsAsync(UserStatsRequest request)
        {

        }
    }
}
