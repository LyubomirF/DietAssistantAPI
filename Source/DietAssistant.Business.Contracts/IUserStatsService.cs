using DietAssistant.Business.Contracts.Models.UserStats.Requests;
using DietAssistant.Business.Contracts.Models.UserStats.Responses;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IUserStatsService
    {
        Task<Result<UserStatsResponse>> SetUserStatsAsync(UserStatsRequest request);
    }
}
