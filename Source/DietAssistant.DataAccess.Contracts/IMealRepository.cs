using DietAssistant.Domain;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IMealRepository : IRepository<Meal>
    {
        Task<Meal> GetMealByIdWithFoodServings(Int32 id);

        Task<IEnumerable<Meal>> GetMealsForDayAsync(DateTime date);

        Task<Meal> GetLastMealAsync(DateTime date);

        Task<Int32> DeleteMealAsync(Meal meal);

        Task<Int32> DeleteFoodServingAsync(Meal meal, FoodServing foodServing);
    }
}
