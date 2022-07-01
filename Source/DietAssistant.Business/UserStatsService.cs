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
    using static UnitConverter;

    public class UserStatsService : IUserStatsService
    {
        private const Double DefaultCarbsPercent = 50;
        private const Double DefaultProteinPercent = 30;
        private const Double DefaultFatPercent = 20;

        private const Double SedentaryMultiplier = 1.2;
        private const Double LightlyActiveMultiplier = 1.375;
        private const Double ActiveMultiplier = 1.55;
        private const Double VeryActiveMultiplier = 1.725;

        private readonly IUserResolverService _userResolverService;
        private readonly IUserStatsRepository _userStatsRepository;
        private readonly IGoalRespository _goalRespository;
        private readonly IProgressLogRepository _progressLogRepository;

        public UserStatsService(
            IUserResolverService userResolverService,
            IUserStatsRepository userStatsRepository,
            IGoalRespository goalRespository,
            IProgressLogRepository progressLogRepository)
        {
            _userResolverService = userResolverService;
            _userStatsRepository = userStatsRepository;
            _goalRespository = goalRespository;
            _progressLogRepository = progressLogRepository;
        }

        public async Task<Result<UserStatsResponse>> GetUserStats()
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var userStats = await _userStatsRepository.GetUserStatsAsync(currentUserId.Value);

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

            var userStats = await _userStatsRepository.GetUserStatsAsync(currentUserId.Value);

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

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
                    request.HeightUnit == HeightUnits.Centimeters ? request.Height : ToCentimeters(request.Height),
                    request.WeightUnit == WeightUnits.Kilograms ? request.Weight : ToKgs(request.Weight),
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
                Weigth = request.Weight,
                LoggedOn = DateTime.Now,
                UserId = currentUserId.Value
            };

            var result = await _userStatsRepository.AddWithGoalAndProgressLogAsync(newUserStats, goal, progressLog);

            return Result.Create(result.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> ChangeHeightUnitAsync(ChangeHeightUnitRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var userStats = await _userStatsRepository.GetUserStatsAsync(currentUserId.Value);

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            if (request.HeightUnit == userStats.HeightUnit)
                return Result.Create(userStats.ToResponse());

            var height = userStats.Height;
            userStats.HeightUnit = request.HeightUnit;

            userStats.Height = userStats.HeightUnit == HeightUnits.Centimeters ? ToCentimeters(height) : ToInches(height);

            await _userStatsRepository.SaveEntityAsync(userStats);

            return Result.Create(userStats.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> ChangeWeightUnitAsync(ChangeWeightUnitRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var userStats = await _userStatsRepository.GetUserStatsAsync(currentUserId.Value);

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var weightUnit = request.WeightUnit;

            if (weightUnit == userStats.WeightUnit)
                return Result.Create(userStats.ToResponse());

            var userId = userStats.UserId;

            userStats.Weight = ConvertWeight(userStats.Weight, weightUnit);
            await _userStatsRepository.SaveEntityAsync(userStats);

            var goal = await _goalRespository.GetGoalByUserId(userId);

            goal.StartWeight = ConvertWeight(goal.StartWeight, weightUnit);
            goal.CurrentWeight = ConvertWeight(goal.CurrentWeight, weightUnit);
            goal.GoalWeight = ConvertWeight(goal.GoalWeight, weightUnit);

            await _goalRespository.SaveEntityAsync(goal);

            var logs = await _progressLogRepository.GetProgressLogsAsync(userId);

            foreach (var log in logs)
                log.Weigth = ConvertWeight(log.Weigth, weightUnit);

            await _progressLogRepository.UpdateRangeAsync(logs);

            return Result.Create(userStats.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> ChangeWeightAsync(ChangeWeightRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var userStats = await _userStatsRepository.GetUserStatsAsync(currentUserId.Value);

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var goal = await _goalRespository.GetGoalByUserId(currentUserId.Value);

            if (goal is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.NotFound, "Goal of user was not found.");

            var weight = request.Weight;

            userStats.Weight = weight;

            await _userStatsRepository.SaveEntityAsync(userStats);

            var heightCm = userStats.HeightUnit == HeightUnits.FeetInches ? ToCentimeters(userStats.Height) : userStats.Height;
            var weightKg = userStats.WeightUnit == WeightUnits.Pounds ? ToKgs(userStats.Weight) : userStats.Weight;

            var dailyCalories = CalculateDailyCalories(
                heightCm,
                weightKg,
                userStats.DateOfBirth.ToAge(DateTime.Today),
                userStats.Gender,
                goal.ActivityLevel, 
                goal.WeeklyGoal);

            goal.CurrentWeight = weight;

            var newNutritionGoal = new NutritionGoal
            {
                Calories = dailyCalories,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentFat = goal.NutritionGoal.PercentFat,
                UserId = currentUserId.Value
            };

            goal.NutritionGoal = newNutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

            var newProgressLog = new ProgressLog
            {
                Weigth = userStats.Weight,
                LoggedOn = DateTime.Now,
                UserId = userStats.UserId
            };

            await _progressLogRepository.SaveEntityAsync(newProgressLog);

            return Result.Create(userStats.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> ChangeHeightAsync(ChangeHeightRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var userStats = await _userStatsRepository.GetUserStatsAsync(currentUserId.Value);

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var goal = await _goalRespository.GetGoalByUserId(currentUserId.Value);

            if (goal is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.NotFound, "Goal of user was not found.");

            var height = request.Height;

            userStats.Height = height;

            await _userStatsRepository.SaveEntityAsync(userStats);

            var heightCm = userStats.HeightUnit == HeightUnits.FeetInches ? ToCentimeters(userStats.Height) : userStats.Height;
            var weightKg = userStats.WeightUnit == WeightUnits.Pounds ? ToKgs(userStats.Weight) : userStats.Weight;

            var dailyCalories = CalculateDailyCalories(
                heightCm,
                weightKg,
                userStats.DateOfBirth.ToAge(DateTime.Today),
                userStats.Gender,
                goal.ActivityLevel,
                goal.WeeklyGoal);

            var newNutritionGoal = new NutritionGoal
            {
                Calories = dailyCalories,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentFat = goal.NutritionGoal.PercentFat,
                UserId = currentUserId.Value
            };

            goal.NutritionGoal = newNutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

            return Result.Create(userStats.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> ChangeGenderAsync(ChangeGenderRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var userStats = await _userStatsRepository.GetUserStatsAsync(currentUserId.Value);

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var goal = await _goalRespository.GetGoalByUserId(currentUserId.Value);

            if (goal is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.NotFound, "Goal of user was not found.");

            var gender = request.Gender;

            userStats.Gender = gender;

            await _userStatsRepository.SaveEntityAsync(userStats);

            var heightCm = userStats.HeightUnit == HeightUnits.FeetInches ? ToCentimeters(userStats.Height) : userStats.Height;
            var weightKg = userStats.WeightUnit == WeightUnits.Pounds ? ToKgs(userStats.Weight) : userStats.Weight;

            var dailyCalories = CalculateDailyCalories(
                heightCm,
                weightKg,
                userStats.DateOfBirth.ToAge(DateTime.Today),
                userStats.Gender,
                goal.ActivityLevel,
                goal.WeeklyGoal);

            var newNutritionGoal = new NutritionGoal
            {
                Calories = dailyCalories,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentFat = goal.NutritionGoal.PercentFat,
                UserId = currentUserId.Value
            };

            goal.NutritionGoal = newNutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

            return Result.Create(userStats.ToResponse());
        }

        public async Task<Result<UserStatsResponse>> ChangeDateOfBirthAsync(ChangeDateOfBirthRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var userStats = await _userStatsRepository.GetUserStatsAsync(currentUserId.Value);

            if (userStats is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats are not set.");

            var goal = await _goalRespository.GetGoalByUserId(currentUserId.Value);

            if (goal is null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.NotFound, "Goal of user was not found.");

            var dateOfBirth = request.DateOfBirth;

            userStats.DateOfBirth = dateOfBirth;

            await _userStatsRepository.SaveEntityAsync(userStats);

            var heightCm = userStats.HeightUnit == HeightUnits.FeetInches ? ToCentimeters(userStats.Height) : userStats.Height;
            var weightKg = userStats.WeightUnit == WeightUnits.Pounds ? ToKgs(userStats.Weight) : userStats.Weight;

            var dailyCalories = CalculateDailyCalories(
                heightCm,
                weightKg,
                userStats.DateOfBirth.ToAge(DateTime.Today),
                userStats.Gender,
                goal.ActivityLevel,
                goal.WeeklyGoal);

            var newNutritionGoal = new NutritionGoal
            {
                Calories = dailyCalories,
                PercentProtein = goal.NutritionGoal.PercentProtein,
                PercentCarbs = goal.NutritionGoal.PercentCarbs,
                PercentFat = goal.NutritionGoal.PercentFat,
                UserId = currentUserId.Value
            };

            goal.NutritionGoal = newNutritionGoal;

            await _goalRespository.SaveEntityAsync(goal);

            return Result.Create(userStats.ToResponse());
        }

        private Double CalculateDailyCalories(
            Double heightCm,
            Double weightKg,
            Int32 age,
            Gender gender,
            ActivityLevel activity,
            WeeklyGoal goal)
        {
            var bmr = gender switch
            {
                Gender.Male => 13.387 * weightKg + 4.799 * heightCm - 5.677 * age + 88.362,
                Gender.Female => 9.247 * weightKg + 3.098 * heightCm - 4.33 * age + 447.539
            };

            var expenditure = activity switch
            {
                ActivityLevel.Sedentary => bmr * SedentaryMultiplier,
                ActivityLevel.LightlyActive => bmr * LightlyActiveMultiplier,
                ActivityLevel.Active => bmr * ActiveMultiplier,
                ActivityLevel.VeryActive => bmr * VeryActiveMultiplier
            };

            return goal switch
            {
                WeeklyGoal.MaintainWeight => expenditure,
                WeeklyGoal.SlowWeightLoss => expenditure - 250,
                WeeklyGoal.ModerateWeightLoss => expenditure - 500,
                WeeklyGoal.IntenseWeightLoss => expenditure - 750,
                WeeklyGoal.ExtremeWeightLoss => expenditure - 1000,
                WeeklyGoal.SlowWeightGain => expenditure + 250,
                WeeklyGoal.ModerateWeightGain => expenditure + 500
            };
        }

        private Double ConvertWeight(Double weight, WeightUnits unit)
            => unit == WeightUnits.Kilograms ? ToKgs(weight) : ToPounds(weight);
    }
}
