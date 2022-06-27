using DietAssistant.Business.Contracts.Models.DietPlanning.Requests;
using DietAssistant.Business.Contracts.Models.DietPlanning.Responses;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IDietPlanningService
    {
        Task<Result<DietPlanResponse>> GetDietPlanAsync(Int32 dietPlanId);

        Task<Result<Int32>> CreateDietPlan(String planName);

        Task<Result<Int32>> DeleteDietPlanAsync(Int32 dietPlanId);

        Task<Result<MealPlanResponse>> AddMealPlanAsync(Int32 dietPlanId, AddUpdateMealRequest request);

        Task<Result<MealPlanResponse>> UpdateMealPlanAsync(Int32 dietPlanId, Int32 mealPlanId, AddUpdateMealRequest request);

        Task<Result<Int32>> DeleteMealPlanAsync(Int32 dietPlanId, Int32 mealPlanId);

        Task<Result<MealPlanResponse>> AddFoodPlanToMeal(Int32 dietPlanId, Int32 mealPlanId, FoodPlanRequest request);

        Task<Result<MealPlanResponse>> UpdateFoodPlan(
            Int32 dietPlanId,
            Int32 mealPlanId,
            Int32 foodPlanId,
            FoodPlanRequest request);

        Task<Result<Int32>> DeleteFoodPlan(Int32 dietPlanId, Int32 mealPlanId, Int32 foodPlanId);
    }
}
