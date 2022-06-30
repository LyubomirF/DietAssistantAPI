using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;

namespace DietAssistant.DataAccess.Repositories
{
    public class UserStatsRepository : Repository<UserStats>, IUserStatsRepository
    {
        public UserStatsRepository(DietAssistantDbContext dbContext) 
            : base(dbContext) { }

        public async Task<UserStats> AddWithGoalAndProgressLogAsync(
            UserStats userStats,
            Goal goal,
            ProgressLog progressLog)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.UsersStats.AddAsync(userStats);
            await _dbContext.Goals.AddAsync(goal);
            await _dbContext.ProgressLogs.AddAsync(progressLog);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return userStats;
        }

        public async Task<Goal> SetNutritionGoalAsync(Goal goal, NutritionGoal nutritionGoal)
        {
            goal.NutritionGoal = nutritionGoal;

            _dbContext.Goals.Update(goal);

            await _dbContext.SaveChangesAsync();

            return goal;
        }

        Task<UserStats> IRepository<UserStats>.GetByIdAsync(int id)
            => GetByIdAsync(id);
    }
}
