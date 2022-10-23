using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Goal.Requests;
using DietAssistant.Business.Contracts.Models.Goal.Responses;
using DietAssistant.Business.Extentions;
using DietAssistant.Business.Helpers;
using DietAssistant.Business.Mappers;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain.Enums;

namespace DietAssistant.Business
{
    using static CalorieHelper;
    using static UnitConvert;

    public class GoalService : IGoalService
    {
        private readonly IUserResolverService _userResolverService;
        private readonly IUserRepository _userRepository;

        public GoalService(
            IUserResolverService userResolverService,
            IUserRepository userRepository)
        {
            _userResolverService = userResolverService;
            _userRepository = userRepository;
        }

        public async Task<Result<GoalResponse>> GetGoalAsync()
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);

            var goal = user.Goal;

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

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);

            var weeklyGoal = ChangeWeeklyGoal(
                request.CurrentWeight,
                user.Goal.GoalWeight,
                user.Goal.WeeklyGoal,
                user.UserStats.WeightUnit);

            var height = user.UserStats.Height;
            var weight = request.CurrentWeight;
            var heightUnit = user.UserStats.HeightUnit;
            var weightUnit = user.UserStats.WeightUnit;
            var age = user.UserStats.DateOfBirth.ToAge(DateTime.Now.Date);

            var calories = CalculateDailyCalories(
                heightUnit == HeightUnit.FeetInches ? ToCentimeters(height) : height,
                weightUnit == WeightUnit.Pounds ? ToKgs(weight) : weight,
                age,
                user.UserStats.Gender,
                user.Goal.ActivityLevel,
                weeklyGoal);

            var updatedUser = await _userRepository.UpdateCurrentWeightAsync(
                user,
                request.CurrentWeight,
                weeklyGoal,
                calories);

            var goal = updatedUser.Goal;

            return Result.Create(goal.ToResponse());
        }

        public async Task<Result<GoalResponse>> ChangeGoalWeightAsync(ChangeGoalWeightRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);
            var weeklyGoal = ChangeWeeklyGoal(
                user.UserStats.Weight,
                request.GoalWeight,
                user.Goal.WeeklyGoal,
                user.UserStats.WeightUnit);

            var height = user.UserStats.Height;
            var weight = user.UserStats.Weight;
            var heightUnit = user.UserStats.HeightUnit;
            var weightUnit = user.UserStats.WeightUnit;
            var age = user.UserStats.DateOfBirth.ToAge(DateTime.Now.Date);

            var calories = CalculateDailyCalories(
                heightUnit == HeightUnit.FeetInches ? ToCentimeters(height) : height,
                weightUnit == WeightUnit.Pounds ? ToKgs(weight) : weight,
                age,
                user.UserStats.Gender,
                user.Goal.ActivityLevel,
                weeklyGoal);

            var updatedUser = await _userRepository.UpdateGoalWeightAsync(user, request.GoalWeight, weeklyGoal, calories);

            var goal = updatedUser.Goal;
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

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);
            var userGoal = user.Goal;

            if (userGoal.WeeklyGoal == weeklyGoal)
                return Result.Create(userGoal.ToResponse());

            var calories = CalculateDailyCalories(
                user.UserStats.GetHeightInCentimeters(),
                user.UserStats.GetWeightInKg(),
                user.UserStats.DateOfBirth.ToAge(DateTime.Now.Date),
                user.UserStats.Gender,
                user.Goal.ActivityLevel,
                weeklyGoal);

            var updatedUser = await _userRepository.UpdateWeeklyGoalAsync(user, weeklyGoal, calories);
            var goal = updatedUser.Goal;

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

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);

            var userStats = user.UserStats;

            if (userStats is null)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var userGoal = user.Goal;

            if (userGoal.ActivityLevel == activityLevel)
                return Result.Create(userGoal.ToResponse());

            var calories = CalculateDailyCalories(
                user.UserStats.GetHeightInCentimeters(),
                user.UserStats.GetWeightInKg(),
                user.UserStats.DateOfBirth.ToAge(DateTime.Now.Date),
                user.UserStats.Gender,
                activityLevel,
                user.Goal.WeeklyGoal);

            var updatedUser = await _userRepository.UpdateActivityLevelAsync(user, activityLevel, calories);
            var goal = updatedUser.Goal;

            return Result.Create(goal.ToResponse());
        }

        public async Task<Result<GoalResponse>> ChangeNutritionGoalAsync(NutritionGoalRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);

            if (user.UserStats is null)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var nutritionGoal = new Domain.NutritionGoal
            {
                Calories = request.Calories,
                PercentCarbs = request.PercentCarbs,
                PercentProtein = request.PercentProtein,
                PercentFat = request.PercentFat,
                ChangedOnUTC = DateTime.UtcNow,
                UserId = currentUserId.Value
            };

            var updatedUser = await _userRepository.UpdateNutritionGoalAsync(user, nutritionGoal);
            var goal = updatedUser.Goal;

            return Result.Create(goal.ToResponse());
        }
    }
}
