using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Responses;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IMealService
    {
        Task<Result<IEnumerable<MealLogResponse>>> GetMealsOnDateAsync(DateTime? dateRequest);

        Task<Result<MealLogResponse>> GetMealById(Int32 id);

        Task<Result<MealLogResponse>> LogMealAsync(LogMealRequest request);

        Task<Result<MealLogResponse>> UpdateMealLogAsync(Int32 id, UpdateMealLogRequest request);

        Task<Result<Int32>> DeleteMealAsync(Int32 id);
    }
}
