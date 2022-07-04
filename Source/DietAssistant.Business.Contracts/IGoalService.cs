using DietAssistant.Business.Contracts.Models.Goal.Requests;
using DietAssistant.Business.Contracts.Models.Goal.Responses;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IGoalService
    {
        Task<Result<GoalResponse>> GetGoalAsync();

        Task<Result<GoalResponse>> ChangeCurrentWeightAsync(ChangeCurrentWeighRequest request);

        Task<Result<GoalResponse>> ChangeGoalWeightAsync(ChangeGoalWeightRequest request);

        Task<Result<GoalResponse>> ChangeWeeklyGoalAsync(ChangeWeeklyGoalRequest request);

        Task<Result<GoalResponse>> ChangeActivityLevelAsync(ChangeActivityLevelRequest request);

        Task<Result<GoalResponse>> ChangeNutritionGoalAsync(NutritionGoalRequest request);
    }
}
