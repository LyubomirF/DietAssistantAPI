using DietAssistant.Business.Contracts.Models.FoodLog.Requests;
using DietAssistant.Business.Contracts.Models.FoodLog.Responses;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IFoodLogService
    {
        Task<Result<NewMealLogResponse>> LogNewMealAsync(LogNewMealRequest request);

        Task<Result<FoodLogResponse>> LogFoodAsync(LogRequest request);
    }
}
