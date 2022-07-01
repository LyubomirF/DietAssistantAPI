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

        public Task<Goal> GetGoalByUserId(int userId)
            => _dbContext.Goals
                .Include(x => x.NutritionGoal)
                .SingleOrDefaultAsync(x => x.UserId == userId);

        Task<Goal> IRepository<Goal>.GetByIdAsync(int id)
            => GetByIdAsync(id);
    }
}
