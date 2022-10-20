using DietAssistant.Business.Contracts;
using DietAssistant.Business.Helpers;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.Domain.Enums;

namespace DietAssistant.Business
{
    using static CalorieHelper;

    public class WeightChangeService : IWeightChangeService
    {
        private const Double MaintainLimitInPounds = 1;
        private const Double MaintainLimitInKg = 0.5;

        private readonly IGoalRespository _goalRespository;
        private readonly IProgressLogRepository _progressLogRepository;
        private readonly IUserStatsRepository _userStatsRepository;

        public WeightChangeService(
            IGoalRespository goalRespository,
            IProgressLogRepository progressLogRepository,
            IUserStatsRepository userStatsRepository)
        {
            _goalRespository = goalRespository;
            _progressLogRepository = progressLogRepository;
            _userStatsRepository = userStatsRepository;
        }

        public async Task HandleWeightChange(Int32 userId, Double weight, Goal goal, UserStats userStats)
        {
            var previousGoal = goal.WeeklyGoal;

            goal.CurrentWeight = weight;
            goal.WeeklyGoal = ChangeWeeklyGoal(
                goal.CurrentWeight,
                goal.GoalWeight,
                previousGoal,
                userStats.WeightUnit);

            var nutritionGoal = new Domain.NutritionGoal
            {
                Calories = CalculateCalories(userStats, goal),
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentFat = goal.NutritionGoal.PercentFat,
                ChangedOnUTC = DateTime.UtcNow,
                UserId = userId
            };

            goal.NutritionGoal = nutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

            var log = new ProgressLog
            {
                Measurement = weight,
                MeasurementType = MeasurementType.Weight,
                LoggedOn = DateTime.Now,
                UserId = userId
            };

            await _progressLogRepository.SaveEntityAsync(log);

            userStats.Weight = weight;
            await _userStatsRepository.SaveEntityAsync(userStats);
        }

        public async Task HandleGoalWeightChange(Int32 userId, Double goalWeight, Goal goal, UserStats userStats)
        {
            var previousGoal = goal.WeeklyGoal;

            goal.GoalWeight = goalWeight;
            goal.WeeklyGoal = ChangeWeeklyGoal(
                goal.CurrentWeight,
                goal.GoalWeight,
                previousGoal,
                userStats.WeightUnit);

            if (previousGoal != goal.WeeklyGoal)
            {
                var nutritionGoal = new Domain.NutritionGoal
                {
                    Calories = CalculateCalories(userStats, goal),
                    PercentCarbs = goal.NutritionGoal.PercentCarbs,
                    PercentProtein = goal.NutritionGoal.PercentProtein,
                    PercentFat = goal.NutritionGoal.PercentFat,
                    ChangedOnUTC = DateTime.UtcNow,
                    UserId = userId
                };

                goal.NutritionGoal = nutritionGoal;
            }

            await _goalRespository.SaveEntityAsync(goal);
        }

        private WeeklyGoal ChangeWeeklyGoal(Double currentWeight, Double goalWeight, WeeklyGoal previousGoal, WeightUnit unit)
        {
            if (ShouldMaintainWeight(currentWeight, goalWeight, unit))
            {
                return WeeklyGoal.MaintainWeight;
            }

            if (goalWeight > currentWeight
                && previousGoal >= WeeklyGoal.MaintainWeight
                && previousGoal <= WeeklyGoal.ExtremeWeightLoss)
            {
                return WeeklyGoal.SlowWeightGain;
            }

            if (goalWeight < currentWeight
                && previousGoal >= WeeklyGoal.SlowWeightGain
                && previousGoal <= WeeklyGoal.ModerateWeightGain)
            {
                return WeeklyGoal.ModerateWeightLoss;
            }

            return previousGoal;
        }

        private bool ShouldMaintainWeight(Double currentWeight, Double goalWeight, WeightUnit unit)
            => unit switch
            {
                WeightUnit.Kilograms =>
                    goalWeight <= currentWeight + MaintainLimitInKg
                    && goalWeight >= currentWeight - MaintainLimitInKg,
                WeightUnit.Pounds =>
                    goalWeight <= currentWeight + MaintainLimitInPounds
                    && goalWeight >= currentWeight - MaintainLimitInPounds
            };
    }
}
