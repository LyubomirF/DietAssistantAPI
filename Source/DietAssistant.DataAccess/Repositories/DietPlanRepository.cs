using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain.DietPlanning;
using Microsoft.EntityFrameworkCore;

#pragma warning disable

namespace DietAssistant.DataAccess.Repositories
{
    public class DietPlanRepository : Repository<DietPlan>, IDietPlanRepository
    {
        public DietPlanRepository(DietAssistantDbContext dbContext)
            : base(dbContext) { }


        public Task<DietPlan> GetDietPlanAsync(Int32 dietPlanId, Int32 userId)
            => _dbContext.DietPlans
                .Include(x => x.MealPlans)
                    .ThenInclude(x => x.FoodPlans)
                .Where(x => x.UserId == userId)
                .SingleOrDefaultAsync(x => x.DietPlanId == dietPlanId);

        Task<DietPlan> IRepository<DietPlan>.GetByIdAsync(Int32 id)
            => GetByIdAsync(id);

        public Task<Int32> DeleteDietPlanAsync(DietPlan dietPlan)
        {
            return DeleteAsync(dietPlan);
        }
        public async Task<Int32> DeleteMealPlanAsync(DietPlan dietPlan, MealPlan mealPlan)
        {
            dietPlan.MealPlans.Remove(mealPlan);

            var result = await _dbContext.SaveChangesAsync();

            return result;
        }

        public async Task<Int32> DeleteFoodPlanAsync(DietPlan dietPlan, MealPlan mealPlan, FoodPlan foodPlan)
        {
            mealPlan.FoodPlans.Remove(foodPlan);

            var result = await _dbContext.SaveChangesAsync();

            return result;
        }

    }
}
