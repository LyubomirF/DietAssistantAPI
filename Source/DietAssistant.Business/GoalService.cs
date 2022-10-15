using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Goal.Requests;
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

    public class GoalService : IGoalService
    {
        private const Double MaintainLimitInPounds = 1;
        private const Double MaintainLimitInKg = 0.5;

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

        public async Task<Result<GoalResponse>> ChangeCurrentWeightAsync(ChangeCurrentWeighRequest request)
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
            var previousGoal = goal.WeeklyGoal;

            goal.CurrentWeight = request.CurrentWeight;
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
                PercentFat = goal.NutritionGoal.PercentFat
            };

            goal.NutritionGoal = nutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

            var log = new ProgressLog
            {
                Measurement = request.CurrentWeight,
                MeasurementType = MeasurementType.Weight,
                LoggedOn = DateTime.Now
            };

            await _progressLogRepository.SaveEntityAsync(log);

            userStats.Weight = request.CurrentWeight;
            await _userStatsRepository.SaveEntityAsync(userStats);

            return Result.Create(goal.ToResponse());
        }

        public async Task<Result<GoalResponse>> ChangeGoalWeightAsync(ChangeGoalWeightRequest request)
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

            var previousGoal = goal.WeeklyGoal;

            goal.GoalWeight = request.GoalWeight;
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
                    ChangedOnUTC = DateTime.UtcNow
                };

                goal.NutritionGoal = nutritionGoal;
            }

            await _goalRespository.SaveEntityAsync(goal);

            return Result.Create(goal.ToResponse());
        }

        public async Task<Result<GoalResponse>> ChangeWeeklyGoalAsync(ChangeWeeklyGoalRequest request)
        {
            if (Enum.TryParse(request.WeeklyGoal, out WeeklyGoal weeklyGoal))
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
                PercentFat = goal.NutritionGoal.PercentFat,
                ChangedOnUTC = DateTime.UtcNow
            };

            goal.NutritionGoal = nutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

            return Result.Create(goal.ToResponse());
        }

        public async Task<Result<GoalResponse>> ChangeActivityLevelAsync(ChangeActivityLevelRequest request)
        {
            if (Enum.TryParse(request.ActivityLevel, out ActivityLevel activityLevel))
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
                PercentFat = goal.NutritionGoal.PercentFat,
                ChangedOnUTC = DateTime.UtcNow
            };

            goal.NutritionGoal = nutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

            return Result.Create(goal.ToResponse());
        }

        public async Task<Result<GoalResponse>> ChangeNutritionGoalAsync(NutritionGoalRequest request)
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

            var nutritionGoal = new Domain.NutritionGoal
            {
                Calories = request.Calories,
                PercentCarbs = request.PercentCarbs,
                PercentProtein = request.PercentProtein,
                PercentFat = request.PercentFat,
                ChangedOnUTC = DateTime.UtcNow
            };

            goal.NutritionGoal = nutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

            return Result.Create(goal.ToResponse());
        }

        private Double CalculateCalories(UserStats userStats, Goal goal)
        {
            var heightCm = userStats.GetHeighInCentimeters();
            var weightKg = userStats.GetWeightInKg();

            return CalculateDailyCalories(
                    heightCm,
                    weightKg,
                    userStats.DateOfBirth.ToAge(DateTime.Today),
                    userStats.Gender,
                    goal.ActivityLevel,
                    goal.WeeklyGoal);
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
