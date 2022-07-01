using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.Domain.Enums;
using Microsoft.EntityFrameworkCore;

#pragma warning disable

namespace DietAssistant.DataAccess.Repositories
{
    public class UserStatsRepository : Repository<UserStats>, IUserStatsRepository
    {
        public UserStatsRepository(DietAssistantDbContext dbContext)
            : base(dbContext) { }

        public Task<UserStats> GetUserStatsAsync(Int32 userId)
           => _dbContext.UsersStats
                 .SingleOrDefaultAsync(x => x.UserId == userId);

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

        Task<UserStats> IRepository<UserStats>.GetByIdAsync(int id)
            => GetByIdAsync(id);

        public async Task<UserStats> UpdateWithWeightChangeAsync(UserStats userStats, Goal goal)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            _dbContext.UsersStats.Update(userStats);

            await _dbContext.SaveChangesAsync();

            _dbContext.Goals.Update(goal);
            await _dbContext.SaveChangesAsync();

            var newProgressLog = new ProgressLog
            {
                Weigth = userStats.Weight,
                LoggedOn = DateTime.Now,
                UserId = userStats.UserId
            };

            _dbContext.ProgressLogs.AddAsync(newProgressLog);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return userStats;
        }

    }
}
