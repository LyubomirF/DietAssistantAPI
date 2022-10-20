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
        private readonly IUserResolverService _userResolverService;
        private readonly IGoalRespository _goalRespository;
        private readonly IProgressLogRepository _progressLogRepository;
        private readonly IUserStatsRepository _userStatsRepository;
        private readonly IWeightChangeService _weightChangeService;

        public GoalService(
            IUserResolverService userResolverService,
            IGoalRespository goalRespository,
            IProgressLogRepository progressLogRepository,
            IUserStatsRepository userStatsRepository,
            IWeightChangeService weightChangeService)
        {
            _userResolverService = userResolverService;
            _goalRespository = goalRespository;
            _progressLogRepository = progressLogRepository;
            _userStatsRepository = userStatsRepository;
            _weightChangeService = weightChangeService;
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

            await _weightChangeService.HandleWeightChange(currentUserId.Value, request.CurrentWeight, goal, userStats);

            var updatedGoal = await _goalRespository.GetGoalByUserIdAsync(currentUserId.Value);

            return Result.Create(updatedGoal.ToResponse());
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

            await _weightChangeService.HandleGoalWeightChange(currentUserId.Value, request.GoalWeight, goal, userStats);

            var updatedGoal = await _goalRespository.GetGoalByUserIdAsync(currentUserId.Value);
            return Result.Create(updatedGoal.ToResponse());
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
                ChangedOnUTC = DateTime.UtcNow,
                UserId = currentUserId.Value
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
                ChangedOnUTC = DateTime.UtcNow,
                UserId = currentUserId.Value
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
                ChangedOnUTC = DateTime.UtcNow,
                UserId = currentUserId.Value
            };

            goal.NutritionGoal = nutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

            return Result.Create(goal.ToResponse());
        }
    }
}
