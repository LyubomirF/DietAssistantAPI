using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Responses;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IMealLogService
    {
        Task<Result<NewMealLogResponse>> LogNewMealAsync(LogMealRequest request);

        Task<Result<FoodLogResponse>> LogFoodAsync(LogFoodRequest request);
    }
}
