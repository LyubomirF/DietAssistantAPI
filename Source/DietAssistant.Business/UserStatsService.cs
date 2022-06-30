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

        public UserStatsService(
            IUserResolverService userResolverService,
            IUserStatsRepository userStatsRepository)
        {
            _userResolverService = userResolverService;
            _userStatsRepository = userStatsRepository;
        }

        public async Task<Result<UserStatsResponse>> SetUserStatsAsync(UserStatsRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var userStats = await _userStatsRepository.GetUserStatsAsync(currentUserId.Value);

            if(userStats is not null)
                return Result
                    .CreateWithError<UserStatsResponse>(EvaluationTypes.InvalidParameters, "User stats already set for user.");

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
                    request.HeightUnit == HeightUnits.Centimeters ? request.Height : request.Height.ToCentimeters(),
                    request.WeightUnit == WeightUnits.Kilograms ? request.Weight : request.Weight.ToKgs(),
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

        private Double CalculateDailyCalories(
            Double height,
            Double weight,
            Int32 age,
            Gender gender,
            ActivityLevel activity,
            WeeklyGoal goal)
        {
            var bmr = gender switch
            {
                Gender.Male => 13.387 * weight + 4.799 * height - 5.677 * age + 88.362,
                Gender.Female => 9.247 * weight + 3.098 * height - 4.33 * age + 447.539
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

    }
}
