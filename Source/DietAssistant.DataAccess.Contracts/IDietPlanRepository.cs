using DietAssistant.Domain.DietPlanning;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IDietPlanRepository : IRepository<DietPlan>
    {
        public Task<DietPlan> GetDietPlanAsync(Int32 dietPlanId, Int32 userId);

        public Task<Int32> DeleteDietPlanAsync(DietPlan dietPlan);

        public Task<Int32> DeleteMealPlanAsync(DietPlan dietPlan, MealPlan mealPlan);

        public Task<Int32> DeleteFoodPlanAsync(DietPlan dietPlan, MealPlan mealPlan, FoodPlan foodPlan);
    }
}
