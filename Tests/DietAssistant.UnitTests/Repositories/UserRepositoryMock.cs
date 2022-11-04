using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.Domain.Enums;
using DietAssistant.UnitTests.Database;
using System;
using System.Collections.Generic;
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
            user.UserStats = userStats;
            user.Goal = goal;

            user.ProgressLogs = new List<ProgressLog>();
            user.ProgressLogs.Add(progressLog);

            return Task.FromResult(user);
        }

        public Task<User> UpdateActivityLevelAsync(User user, ActivityLevel activityLevel, double calories)
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

            return Task.FromResult(user);
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
            var userStats = user.UserStats;
            var goal = user.Goal;

            userStats.DateOfBirth = dateOfBirth;

            var newNutritionGoal = new NutritionGoal
            {
                Calories = calories,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentFat = goal.NutritionGoal.PercentFat,
                UserId = user.UserId
            };

            goal.NutritionGoal = newNutritionGoal;

            return Task.FromResult(user);
        }

        public Task<User> UpdateGenderAsync(User user, Gender gender, double calories)
        {
            var userStats = user.UserStats;
            var goal = user.Goal;

            userStats.Gender = gender;

            var newNutritionGoal = new NutritionGoal
            {
                Calories = calories,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentFat = goal.NutritionGoal.PercentFat,
                UserId = user.UserId
            };

            goal.NutritionGoal = newNutritionGoal;

            return Task.FromResult(user);
        }

        public Task<User> UpdateGoalWeightAsync(User user, double goalWeight, WeeklyGoal weeklyGoal, double calories)
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

            return Task.FromResult(user);
        }

        public Task<User> UpdateHeightAsync(User user, double height, double calories)
        {
            var userStats = user.UserStats;
            userStats.Height = height;

            var goal = user.Goal;

            var newNutritionGoal = new NutritionGoal
            {
                Calories = calories,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentFat = goal.NutritionGoal.PercentFat,
                UserId = user.UserId
            };

            goal.NutritionGoal = newNutritionGoal;

            return Task.FromResult(user);
        }

        public Task<User> UpdateHeightUnitAsync(User user, HeightUnit heightUnit)
        {
            var userStats = user.UserStats;

            userStats.Height = heightUnit == HeightUnit.Centimeters
                ? userStats.GetHeightInCentimeters()
                : userStats.GetHeightInInches();

            userStats.HeightUnit = heightUnit;

            return Task.FromResult(user);
        }

        public Task<User> UpdateNutritionGoalAsync(User user, NutritionGoal nutritionGoal)
        {
            user.Goal.NutritionGoal = nutritionGoal;

            return Task.FromResult(user);
        }

        public Task<User> UpdateWeeklyGoalAsync(User user, WeeklyGoal weeklyGoal, double calories)
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

            return Task.FromResult(user);
        }

        public Task<User> UpdateWeightUnitAsync(User user, WeightUnit weightUnit, Func<double, WeightUnit, double> converter)
        {
            var userStats = user.UserStats;
            userStats.Weight = converter(userStats.Weight, weightUnit);
            userStats.WeightUnit = weightUnit;

            var goal = user.Goal;
            goal.StartWeight = converter(goal.StartWeight, weightUnit);
            goal.CurrentWeight = converter(goal.CurrentWeight, weightUnit);
            goal.GoalWeight = converter(goal.GoalWeight, weightUnit);

            var logs = user.ProgressLogs;

            foreach (var log in logs)
                log.Measurement = converter(log.Measurement, weightUnit);

            return Task.FromResult(user);
        }
    }
}
