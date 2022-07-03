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

            if (goal is null)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.NotFound, "Goal was not found.");

            return Result.Create(goal.ToResponse());
        }

        public async Task<Result<GoalResponse>> ChangeCurrentWeightAsync(Double currentWeightRequest)
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

            goal.CurrentWeight = currentWeightRequest;

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
                Measurement = currentWeightRequest,
                MeasurementType = MeasurementType.Weight,
                LoggedOn = DateTime.Now
            };

            await _progressLogRepository.SaveEntityAsync(log);

            return Result.Create(goal.ToResponse());
        }

        public async Task<Result<GoalResponse>> ChangeGoalWeightAsync(Double goalWeightRequest)
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

            var previousWeeklyGoal = goal.WeeklyGoal;

            goal.GoalWeight = goalWeightRequest;
            goal.WeeklyGoal = ChangeWeeklyGoal(goal.CurrentWeight, goal.GoalWeight, goal.WeeklyGoal);

            if(previousWeeklyGoal != goal.WeeklyGoal)
            {
                var nutritionGoal = new Domain.NutritionGoal
                {
                    Calories = CalculateCalories(userStats, goal),
                    PercentCarbs = goal.NutritionGoal.PercentCarbs,
                    PercentProtein = goal.NutritionGoal.PercentProtein,
                    PercentFat = goal.NutritionGoal.PercentFat
                };

                goal.NutritionGoal = nutritionGoal;
            }    

            await _goalRespository.SaveEntityAsync(goal);

            return Result.Create(goal.ToResponse());
        }

        public async Task<Result<GoalResponse>> ChangeWeeklyGoalAsync(String weeklyGoalRequest)
        {
            if (Enum.TryParse(weeklyGoalRequest, out WeeklyGoal weeklyGoal))
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.InvalidParameters, "Invalid weekly goal value.");

            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var userStats = await _userStatsRepository.GetUserStatsAsync(currentUserId.Value);

            if (userStats is null)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var goal = await _goalRespository.GetGoalByUserIdAsync(currentUserId.Value);

            if (goal.WeeklyGoal == weeklyGoal)
                return Result.Create(goal.ToResponse());

            goal.WeeklyGoal = weeklyGoal;

            var nutritionGoal = new Domain.NutritionGoal
            {
                Calories = CalculateCalories(userStats, goal),
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentFat = goal.NutritionGoal.PercentFat
            };

            goal.NutritionGoal = nutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

            return Result.Create(goal.ToResponse());
        }

        public async Task<Result<GoalResponse>> ChangeActivityLevelAsync(String activityLevelRequest)
        {
            if (Enum.TryParse(activityLevelRequest, out ActivityLevel activityLevel))
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.InvalidParameters, "Invalid activity level value.");

            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var userStats = await _userStatsRepository.GetUserStatsAsync(currentUserId.Value);

            if (userStats is null)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var goal = await _goalRespository.GetGoalByUserIdAsync(currentUserId.Value);

            if (goal.ActivityLevel == activityLevel)
                return Result.Create(goal.ToResponse());

            goal.ActivityLevel = activityLevel;

            var nutritionGoal = new Domain.NutritionGoal
            {
                Calories = CalculateCalories(userStats, goal),
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentFat = goal.NutritionGoal.PercentFat
            };

            goal.NutritionGoal = nutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

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

        private WeeklyGoal ChangeWeeklyGoal(Double currentWeight, Double goalWeight, WeeklyGoal currentWeeklyGoal)
        {
            if (goalWeight > currentWeight
                && (currentWeeklyGoal >= WeeklyGoal.MaintainWeight
                    && currentWeeklyGoal <= WeeklyGoal.ExtremeWeightLoss))
            {
                return WeeklyGoal.SlowWeightGain;
            }

            if (goalWeight < currentWeight
                && (currentWeeklyGoal >= WeeklyGoal.SlowWeightGain
                    && currentWeeklyGoal <= WeeklyGoal.ModerateWeightGain))
            {
                return WeeklyGoal.ModerateWeightLoss;
            }

            return currentWeeklyGoal;
        }
    }
}
