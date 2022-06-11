using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Responses;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IMealLogService
    {
        Task<Result<MealLogResponse>> GetMealById(Int32 id);

        Task<Result<MealLogResponse>> LogMealAsync(LogMealRequest request);

        Task<Result<MealLogResponse>> UpdateMealLogAsync(Int32 id, UpdateMealLogRequest request);

        Task<Result<Int32>> DeleteMealAsync(Int32 id);

        Task<Result<FoodLogResponse>> LogFoodAsync(Int32 mealdId, LogFoodRequest request);
    }
}
