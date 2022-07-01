using DietAssistant.Domain;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IUserStatsRepository : IRepository<UserStats>
    {
        Task<UserStats> AddWithGoalAndProgressLogAsync(
            UserStats userStats,
            Goal goal,
            ProgressLog progressLog);

        Task<UserStats> UpdateWithWeightChangeAsync(UserStats userStats, Goal goal);

        Task<UserStats> GetUserStatsAsync(Int32 userId);
    } 
}
