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
                Calories = CalculateDailyCalories(userStats, goal),
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
    }
}
