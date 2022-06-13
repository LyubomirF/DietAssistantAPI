using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8603

namespace DietAssistant.DataAccess.Repositories
{
    public class MealRepository : Repository<Meal>, IMealRepository
    {
        public MealRepository(DietAssistantDbContext dbContext)
            : base(dbContext) { }

        public async Task<Meal> GetMealByIdWithFoodServings(int id, int userId)
            => await _dbContext.Meals
                .Include(x => x.FoodServings)
                .SingleOrDefaultAsync(x => x.MealId == id && x.UserId == userId);

        public async Task<Meal> GetLastMealAsync(DateTime date, Int32 userId)
             => await _dbContext.Meals
                .Include(x => x.FoodServings)
                .Where(x => x.EatenOn == date && x.UserId == userId)
                .OrderBy(x => x.Order)
                .LastOrDefaultAsync();

        public async Task<IEnumerable<Meal>> GetMealsForDayAsync(DateTime dateTime, Int32 userId)
            => await _dbContext.Meals
                .Include(x => x.FoodServings)
                .Where(x => x.EatenOn == dateTime && x.UserId == userId)
                .OrderBy(x => x.Order)
                .ToListAsync();

        Task<Meal> IRepository<Meal>.GetByIdAsync(int id)
            => GetByIdAsync(id);

        public async Task<Int32> DeleteMealAsync(Meal meal)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            var mealEatenOn = meal.EatenOn;
            var userId = meal.UserId;

            var result = await DeleteAsync(meal);

            var loggedMeals = await _dbContext.Meals
                .Where(x => x.EatenOn == mealEatenOn && x.UserId == userId)
                .OrderBy(x => x.Order)
                .ToListAsync();

            var mealOrder = 1;
            foreach (var loggedMeal in loggedMeals)
                loggedMeal.Order = mealOrder++;

            _dbContext.UpdateRange(loggedMeals);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return result;
        }

        public async Task<Int32> DeleteFoodServingAsync(Meal meal, FoodServing foodServing)
        {
            meal.FoodServings.Remove(foodServing);

            return await _dbContext.SaveChangesAsync();
        }
    }
}
