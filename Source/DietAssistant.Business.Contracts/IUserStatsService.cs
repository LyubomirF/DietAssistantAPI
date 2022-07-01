using DietAssistant.Business.Contracts.Models.UserStats.Requests;
using DietAssistant.Business.Contracts.Models.UserStats.Responses;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IUserStatsService
    {
        Task<Result<UserStatsResponse>> GetUserStats();

        Task<Result<UserStatsResponse>> SetUserStatsAsync(UserStatsRequest request);

        Task<Result<UserStatsResponse>> ChangeHeightUnitAsync(ChangeHeightUnitRequest heightUnit);

        Task<Result<UserStatsResponse>> ChangeWeightUnitAsync(ChangeWeightUnitRequest request);

        Task<Result<UserStatsResponse>> ChangeWeightAsync(ChangeWeightRequest request);

        Task<Result<UserStatsResponse>> ChangeHeightAsync(ChangeHeightRequest request);

        Task<Result<UserStatsResponse>> ChangeGenderAsync(ChangeGenderRequest request);

        Task<Result<UserStatsResponse>> ChangeDateOfBirthAsync(ChangeDateOfBirthRequest request);

    }
}
