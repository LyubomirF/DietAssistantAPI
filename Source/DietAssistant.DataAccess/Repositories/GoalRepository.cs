using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using Microsoft.EntityFrameworkCore;

#pragma warning disable

namespace DietAssistant.DataAccess.Repositories
{
    public class GoalRepository : Repository<Goal>, IGoalRespository
    {
        public GoalRepository(DietAssistantDbContext dbContext) 
            : base(dbContext) { }

        public Task<Goal> GetGoalByUserIdAsync(Int32 userId)
            => _dbContext.Goals
                .Include(x => x.NutritionGoal)
                .SingleOrDefaultAsync(x => x.UserId == userId);

        public async Task<Goal> UpdateWithNutritionGoal(Goal goal, NutritionGoal nutritionGoal)
        {
            goal.NutritionGoal = nutritionGoal;
            _dbContext.Set<Goal>().Update(goal);
            await _dbContext.SaveChangesAsync();

            return goal;
        }

        Task<Goal> IRepository<Goal>.GetByIdAsync(int id)
            => GetByIdAsync(id);
    }
}
