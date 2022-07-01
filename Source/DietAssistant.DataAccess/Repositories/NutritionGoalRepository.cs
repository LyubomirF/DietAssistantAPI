using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;

namespace DietAssistant.DataAccess.Repositories
{
    public class NutritionGoalRepository : Repository<NutritionGoal>, INutritionGoalRepository
    {
        public NutritionGoalRepository(DietAssistantDbContext dbContext) 
            : base(dbContext) { }

        //public Task<NutritionGoal> GetLatestNutritionGoalByUserId(Int32 userId)
        //{
        //    _dbContext.NutritionGoals.SingleOrDefault()
        //}

        Task<NutritionGoal> IRepository<NutritionGoal>.GetByIdAsync(Int32 id)
            => GetByIdAsync(id);
    }
}
