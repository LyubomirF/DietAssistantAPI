using DietAssistant.Business.Contracts.Models.FoodServing.Requests;
using DietAssistant.Business.Contracts.Models.FoodServing.Responses;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IFoodServingService
    {
        Task<Result<FoodServingResponse>> LogFoodServingAsync(Int32 mealdId, LogUpdateFoodServingRequest request);

        Task<Result<FoodServingResponse>> UpdateFoodServingLogAsync(Int32 mealId, Int32 foodServingId, LogUpdateFoodServingRequest request);

        Task<Result<Int32>> DeleteFoodServingLogAsync(Int32 mealId, Int32 foodServingId);
    }
}
