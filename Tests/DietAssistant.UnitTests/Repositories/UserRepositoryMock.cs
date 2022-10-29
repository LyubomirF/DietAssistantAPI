using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.Domain.Enums;
using DietAssistant.UnitTests.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable

namespace DietAssistant.UnitTests.Repositories
{
    using static DatabaseMock;

    internal class UserRepositoryMock : IRepository<User>, IUserRepository
    {
        public Task<User> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByIdAsync(int userId) 
            => Task.FromResult(Users.SingleOrDefault(x => x.UserId == userId));

        public Task SaveEntityAsync(User entity)
        {
            return Task.FromResult(1);
        }

        public Task<User> SetStatsAsync(User user, UserStats userStats, Goal goal, ProgressLog progressLog)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateActivityLevelAsync(User user, ActivityLevel activityLevel, double calories)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateCurrentWeightAsync(User user, double weight, WeeklyGoal weeklyGoal, double updatedCalories)
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
                LoggedOn = DateTime.Now,
                UserId = user.UserId
            };

            user.UserStats.Weight = weight;
            user.ProgressLogs.Add(log);

            return Task.FromResult(user);
        }

        public Task<User> UpdateDateOfBirthAsync(User user, DateTime dateOfBirth, double calories)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateGenderAsync(User user, Gender gender, double calories)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateGoalWeightAsync(User user, double goalWeight, WeeklyGoal weeklyGoal, double calories)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateHeightAsync(User user, double height, double calories)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateHeightUnitAsync(User user, HeightUnit heightUnit)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateNutritionGoalAsync(User user, NutritionGoal nutritionGoal)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateWeeklyGoalAsync(User user, WeeklyGoal weeklyGoal, double calories)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateWeightUnitAsync(User user, WeightUnit weightUnit, Func<double, WeightUnit, double> converter)
        {
            throw new NotImplementedException();
        }
    }
}
