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

        Task<User> SetStatsAsync(User user, UserStats userStats, Goal goal, ProgressLog progressLog);

        Task<User> UpdateHeightUnitAsync(User user, HeightUnit heightUnit);

        Task<User> UpdateWeightUnitAsync(User user, WeightUnit weightUnit, Func<Double, WeightUnit, Double> converter);

        Task<User> UpdateHeightAsync(User user, Double height, Double calories);

        Task<User> UpdateGenderAsync(User user, Gender gender, Double calories);

        Task<User> UpdateDateOfBirthAsync(User user, DateTime dateOfBirth, Double calories);
    }
}
