using DietAssistant.Business.Contracts.Models.UserStats.Requests;
using DietAssistant.Domain;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IUserStatsRepository : IRepository<UserStats>
    {
        Task<UserStats> AddWithGoalAndProgressLogAsync(
            UserStats userStats,
            Goal goal,
            ProgressLog progressLog);

        Task<Goal> SetNutritionGoalAsync(Goal goal, NutritionGoal nutritionGoal);
    }
}
