using DietAssistant.Domain;
using DietAssistant.Domain.Enums;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByEmailAsync(String email);

        Task<User> GetUserByIdAsync(Int32 userId);

        Task<User> UpdateCurrentWeightAsync(
            User user,
            Double weight,
            WeeklyGoal weeklyGoal,
            Double updatedCalories);

        Task<User> UpdateGoalWeightAsync(
            User user,
            Double goalWeight,
            WeeklyGoal weeklyGoal,
            Double calories);

        Task<User> UpdateWeeklyGoalAsync(User user, WeeklyGoal weeklyGoal, Double calories);

        Task<User> UpdateActivityLevelAsync(User user, ActivityLevel activityLevel, Double calories);

        Task<User> UpdateNutritionGoalAsync(User user, NutritionGoal nutritionGoal);
    }
}
