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

        public async Task<Meal> GetLastMealAsync(DateTime date)
             => await _dbContext.Meals
                .Include(x => x.FoodServings)
                .Where(x => x.EatenOn == date)
                .OrderBy(x => x.Order)
                .LastOrDefaultAsync();

        public async Task<IEnumerable<Meal>> GetMealsForDayAsync(DateTime dateTime)
            => await _dbContext.Meals
                .Include(x => x.FoodServings)
                .Where(x => x.EatenOn == dateTime)
                .OrderBy(x => x.Order)
                .ToListAsync();

        Task<Meal> IRepository<Meal>.GetByIdAsync(int id)
            => GetByIdAsync(id);
    }
}
