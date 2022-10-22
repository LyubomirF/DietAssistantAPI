using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.UserStats.Requests;
using DietAssistant.Business.Contracts.Models.UserStats.Responses;
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
    using static UnitConvert;

    public class UserStatsService : IUserStatsService
    {
        private const Double DefaultCarbsPercent = 50;
        private const Double DefaultProteinPercent = 30;
        private const Double DefaultFatPercent = 20;

        private readonly IUserResolverService _userResolverService;
        private readonly IUserRepository _userRepository;

        public UserStatsService(
            IUserResolverService userResolverService,
            IUserRepository userRepository)
        {
            _userResolverService = userResolverService;
            _userRepository = userRepository;
        }

        public async Task<Result<UserStatsResponse>> GetUserStats()
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);
            var userStats = user.UserStats;

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            return Result.Create(userStats.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> SetUserStatsAsync(UserStatsRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);

            if (user.UserStats is not null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are already set.");

            var newUserStats = new UserStats
            {
                Height = request.Height,
                Weight = request.Weight,
                WeightUnit = request.WeightUnit,
                HeightUnit = request.HeightUnit,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                UserId = currentUserId.Value,
            };

            var nutritionGoal = new NutritionGoal
            {
                Calories = CalculateDailyCalories(
                    request.HeightUnit == HeightUnit.Centimeters ? request.Height : ToCentimeters(request.Height),
                    request.WeightUnit == WeightUnit.Kilograms ? request.Weight : ToKgs(request.Weight),
                    request.DateOfBirth.ToAge(DateTime.Today),
                    request.Gender,
                    ActivityLevel.Sedentary,
                    WeeklyGoal.MaintainWeight),
                PercentCarbs = DefaultCarbsPercent,
                PercentProtein = DefaultProteinPercent,
                PercentFat = DefaultFatPercent,
                ChangedOnUTC = DateTime.UtcNow,
                UserId = currentUserId.Value
            };

            var goal = new Goal
            {
                StartWeight = request.Weight,
                StartDate = DateTime.Today,
                CurrentWeight = request.Weight,
                GoalWeight = request.Weight,
                WeeklyGoal = WeeklyGoal.MaintainWeight,
                ActivityLevel = ActivityLevel.Sedentary,
                NutritionGoal = nutritionGoal,
                UserId = currentUserId.Value
            };

            var progressLog = new ProgressLog
            {
                MeasurementType = MeasurementType.Weight,
                Measurement = request.Weight,
                LoggedOn = DateTime.Now,
                UserId = currentUserId.Value
            };

            var updatedUser = await _userRepository.SetStatsAsync(user, newUserStats, goal, progressLog);
            var userStats = updatedUser.UserStats;

            return Result.Create(userStats.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> ChangeHeightUnitAsync(ChangeHeightUnitRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);
            var userStats = user.UserStats;

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            if (request.HeightUnit == userStats.HeightUnit)
                return Result.Create(userStats.ToResponse());

            var updatedUserStats = (await _userRepository.UpdateHeightUnitAsync(user, request.HeightUnit)).UserStats;

            return Result.Create(updatedUserStats.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> ChangeWeightUnitAsync(ChangeWeightUnitRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);
            var userStats = user.UserStats;

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var weightUnit = request.WeightUnit;

            if (weightUnit == userStats.WeightUnit)
                return Result.Create(userStats.ToResponse());

            var updatedUser = await _userRepository.UpdateWeightUnitAsync(user, request.WeightUnit, ConvertWeight);
            
            return Result.Create(updatedUser.UserStats.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> ChangeWeightAsync(ChangeWeightRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);

            var userStats = user.UserStats;

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var weeklyGoal = ChangeWeeklyGoal(
                request.Weight,
                user.Goal.GoalWeight,
                user.Goal.WeeklyGoal,
                user.UserStats.WeightUnit);

            var height = user.UserStats.Height;
            var weight = request.Weight;
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
                request.Weight,
                weeklyGoal,
                calories);

            return Result.Create(updatedUser.UserStats.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> ChangeHeightAsync(ChangeHeightRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);
            var userStats = user.UserStats;

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var height = request.Height;
            var heightUnit = user.UserStats.HeightUnit;
            var age = user.UserStats.DateOfBirth.ToAge(DateTime.Now.Date);

            var calories = CalculateDailyCalories(
                heightUnit == HeightUnit.FeetInches ? ToCentimeters(height) : height,
                user.UserStats.GetWeightInKg(),
                age,
                user.UserStats.Gender,
                user.Goal.ActivityLevel,
                user.Goal.WeeklyGoal);

            var updatedUser = await _userRepository.UpdateHeightAsync(user, height, calories);

            return Result.Create(updatedUser.UserStats.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> ChangeGenderAsync(ChangeGenderRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);
            var userStats = user.UserStats;

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var age = user.UserStats.DateOfBirth.ToAge(DateTime.Now.Date);

            var calories = CalculateDailyCalories(
                user.UserStats.GetHeightInCentimeters(),
                user.UserStats.GetWeightInKg(),
                age,
                request.Gender,
                user.Goal.ActivityLevel,
                user.Goal.WeeklyGoal);

            var updatedUser = await _userRepository.UpdateGenderAsync(user, request.Gender, calories);

            return Result.Create(updatedUser.UserStats.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> ChangeDateOfBirthAsync(ChangeDateOfBirthRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);
            var userStats = user.UserStats;

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var age = request.DateOfBirth.ToAge(DateTime.Now.Date);

            var calories = CalculateDailyCalories(
                user.UserStats.GetHeightInCentimeters(),
                user.UserStats.GetWeightInKg(),
                age,
                user.UserStats.Gender,
                user.Goal.ActivityLevel,
                user.Goal.WeeklyGoal);

            var updatedUser = await _userRepository.UpdateDateOfBirthAsync(user, request.DateOfBirth, calories);
            return Result.Create(updatedUser.UserStats.ToResponse());
        }

        private static Double ConvertWeight(Double weight, WeightUnit unit)
            => unit == WeightUnit.Kilograms ? ToKgs(weight) : ToPounds(weight);
    }
}
