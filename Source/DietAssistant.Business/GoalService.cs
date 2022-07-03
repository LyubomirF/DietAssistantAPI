using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Goal.Responses;
using DietAssistant.Business.Extentions;
using DietAssistant.Business.Helpers;
using DietAssistant.Business.Mappers;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.Domain.Enums;

namespace DietAssistant.Business
{
    using static CalorieHelper;
    using static UnitConverter;

    public class GoalService : IGoalService
    {
        private readonly IUserResolverService _userResolverService;
        private readonly IGoalRespository _goalRespository;
        private readonly IProgressLogRepository _progressLogRepository;
        private readonly IUserStatsRepository _userStatsRepository;

        public GoalService(
            IUserResolverService userResolverService,
            IGoalRespository goalRespository,
            IProgressLogRepository progressLogRepository,
            IUserStatsRepository userStatsRepository)
        {
            _userResolverService = userResolverService;
            _goalRespository = goalRespository;
            _progressLogRepository = progressLogRepository;
            _userStatsRepository = userStatsRepository;
        }

        public async Task<Result<GoalResponse>> GetGoalAsync()
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var goal = await _goalRespository.GetGoalByUserIdAsync(currentUserId.Value);

            if(goal is null)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.NotFound, "Goal was not found.");

            return Result.Create(goal.ToResponse());
        }

        public async Task<Result<GoalResponse>> ChangeCurrentWeightAsync(Double currentWeight)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var userStats = await _userStatsRepository.GetUserStatsAsync(currentUserId.Value);

            if (userStats is null)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var goal = await _goalRespository.GetGoalByUserIdAsync(currentUserId.Value);

            goal.CurrentWeight = currentWeight;

            var nutritionGoal = new Domain.NutritionGoal
            {
                Calories = CalculateCalories(userStats, goal),
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentFat = goal.NutritionGoal.PercentFat
            };

            goal.NutritionGoal = nutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

            var log = new ProgressLog
            {
                Measurement = currentWeight,
                MeasurementType = MeasurementType.Weight,
                LoggedOn = DateTime.Now
            };

            await _progressLogRepository.SaveEntityAsync(log);

            return Result.Create(goal.ToResponse());
        }


        private Double CalculateCalories(UserStats userStats, Goal goal)
        {
            var heightCm = userStats.HeightUnit == HeightUnit.FeetInches ? ToCentimeters(userStats.Height) : userStats.Height;
            var weightKg = userStats.WeightUnit == WeightUnit.Pounds ? ToKgs(userStats.Weight) : userStats.Weight;

            return CalculateDailyCalories(
                    heightCm,
                    weightKg,
                    userStats.DateOfBirth.ToAge(DateTime.Today),
                    userStats.Gender,
                    goal.ActivityLevel,
                    goal.WeeklyGoal);
        }
    }
}
