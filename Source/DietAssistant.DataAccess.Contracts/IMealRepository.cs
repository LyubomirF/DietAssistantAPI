using DietAssistant.Domain;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IMealRepository : IRepository<Meal>
    {
        Task<Meal> GetMealByIdWithFoodServingsAsync(Int32 mealId, Int32 userId);

        Task<IEnumerable<Meal>> GetMealsForDayAsync(DateTime date, Int32 userId);

        Task<Meal> GetLastMealAsync(DateTime date, Int32 userId);

        Task<Int32> DeleteMealAsync(Meal meal);

        Task<Int32> DeleteFoodServingAsync(Meal meal, FoodServing foodServing);
    }
}
