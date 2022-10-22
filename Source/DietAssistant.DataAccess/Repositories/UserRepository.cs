using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.Domain.Enums;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8619
#pragma warning disable CS8603

namespace DietAssistant.DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DietAssistantDbContext dbContext)
            : base(dbContext) { }

        public async Task<User> GetUserByEmailAsync(String email)
        {
            if (email is null)
                return null;

            return await _dbContext.Users
               .SingleOrDefaultAsync(x => x.Email == email.Trim());
        }

        public async Task<User> GetUserByIdAsync(Int32 userId)
        {
            return await _dbContext.Users
                .Include(x => x.Goal)
                    .ThenInclude(x => x.NutritionGoal)
                .Include(x => x.ProgressLogs.Where(x => x.LoggedOn >= DateTime.Now.AddDays(-30)))
                .Include(x => x.DietPlans)
                .Include(x => x.Meals)
                .Include(x => x.UserStats)
                .SingleOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<User> UpdateCurrentWeightAsync(
            User user, 
            Double weight,
            WeeklyGoal weeklyGoal,
            Double updatedCalories)
        {
            var goal = user.Goal;

            goal.CurrentWeight = weight;
            goal.WeeklyGoal = weeklyGoal;

            var nutritionGoal = new NutritionGoal
            {
                Calories = updatedCalories,
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentFat = goal.NutritionGoal.PercentFat,
                ChangedOnUTC = DateTime.Now.Date,
                UserId = user.UserId
            };

            goal.NutritionGoal = nutritionGoal;

            var log = new ProgressLog
            {
                Measurement = weight,
                MeasurementType = MeasurementType.Weight,
                LoggedOn = DateTime.Now.Date,
                UserId = user.UserId
            };

            user.UserStats.Weight = weight;
            user.ProgressLogs.Add(log);

            await SaveEntityAsync(user);

            return user;
        }

        public async Task<User> UpdateGoalWeightAsync(
            User user,
            Double goalWeight,
            WeeklyGoal weeklyGoal,
            Double calories)
        {
            var goal = user.Goal;

            var previousGoal = goal.WeeklyGoal;

            goal.GoalWeight = goalWeight;
            goal.WeeklyGoal = weeklyGoal;

            if (previousGoal != goal.WeeklyGoal)
            {
                var nutritionGoal = new NutritionGoal
                {
                    Calories = calories,
                    PercentCarbs = goal.NutritionGoal.PercentCarbs,
                    PercentProtein = goal.NutritionGoal.PercentProtein,
                    PercentFat = goal.NutritionGoal.PercentFat,
                    ChangedOnUTC = DateTime.UtcNow.Date,
                    UserId = user.UserId
                };

                goal.NutritionGoal = nutritionGoal;
            }

            await SaveEntityAsync(user);

            return user;
        }

        public async Task<User> UpdateWeeklyGoalAsync(User user, WeeklyGoal weeklyGoal, Double calories)
        {
            var goal = user.Goal;

            goal.WeeklyGoal = weeklyGoal;

            var nutritionGoal = new NutritionGoal
            {
                Calories = calories,
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentFat = goal.NutritionGoal.PercentFat,
                ChangedOnUTC = DateTime.UtcNow,
                UserId = user.UserId
            };

            goal.NutritionGoal = nutritionGoal;

            await SaveEntityAsync(user);
            return user;
        }

        public async Task<User> UpdateActivityLevelAsync(User user, ActivityLevel activityLevel, Double calories)
        {
            var goal = user.Goal;

            goal.ActivityLevel = activityLevel;

            var nutritionGoal = new NutritionGoal
            {
                Calories = calories,
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentFat = goal.NutritionGoal.PercentFat,
                ChangedOnUTC = DateTime.UtcNow,
                UserId = user.UserId
            };

            goal.NutritionGoal = nutritionGoal;

            await SaveEntityAsync(user);
            return user;
        }

        public async Task<User> UpdateNutritionGoalAsync(User user, NutritionGoal nutritionGoal)
        {
            user.Goal.NutritionGoal = nutritionGoal;

            await SaveEntityAsync(user);

            return user;
        }

        Task<User> IRepository<User>.GetByIdAsync(int id)
            => GetByIdAsync(id);
    }
}
