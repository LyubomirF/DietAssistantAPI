using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Paging;
using DietAssistant.Business.Contracts.Models.ProgressLog.Requests;
using DietAssistant.Business.Contracts.Models.ProgressLog.Responses;
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

    public class ProgressLogService : IProgressLogService
    {
        private readonly IUserResolverService _userResolverService;
        private readonly IProgressLogRepository _progressLogRepository;
        private readonly IUserRepository _userRepository;

        public ProgressLogService(
            IUserResolverService userResolverService,
            IUserRepository userRepository,
            IProgressLogRepository progressLogRepository)
        {
            _progressLogRepository = progressLogRepository;
            _userResolverService = userResolverService;
            _userRepository = userRepository;
        }

        public async Task<Result<PagedResult<ProgressLogResponse>>> GetProgressLogsPagedAsync(ProgressLogFilterRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<PagedResult<ProgressLogResponse>>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            if (!Enum.TryParse(request.MeasurementType, out MeasurementType measurementType))
                return Result
                    .CreateWithError<PagedResult<ProgressLogResponse>>(EvaluationTypes.InvalidParameters, "Invalid measurement type.");

            var (progressLogs, totalCount) =
                await _progressLogRepository.GetProgressLogPagedAsync(
                    currentUserId.Value,
                    measurementType,
                    request.PeriodStart,
                    request.PeriodEnd,
                    request.Page,
                    request.PageSize);

            return Result.Create(progressLogs.ToPagedResult(request.Page, request.PageSize, totalCount));
        }

        public async Task<Result<ProgressLogResponse>> AddProgressLogAsync(AddProgressLogRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<ProgressLogResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            if (!Enum.TryParse(request.MeasurementType, out MeasurementType measurementType))
                return Result
                    .CreateWithError<ProgressLogResponse>(EvaluationTypes.InvalidParameters, "Invalid measurement type.");

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);
            var userStats = user.UserStats;

            if (userStats is null)
                return Result
                    .CreateWithError<ProgressLogResponse>(EvaluationTypes.Failed, "User stats are not set.");

            if (measurementType == MeasurementType.Weight)
            {
                var weeklyGoal = ChangeWeeklyGoal(
                    request.Measurement,
                    user.Goal.GoalWeight,
                    user.Goal.WeeklyGoal,
                    user.UserStats.WeightUnit);

                var height = user.UserStats.Height;
                var weight = request.Measurement;
                var heightUnit = user.UserStats.HeightUnit;
                var weightUnit = user.UserStats.WeightUnit;
                var age = user.UserStats.DateOfBirth.ToAge(DateTime.Today);

                var calories = CalculateDailyCalories(
                    heightUnit == HeightUnit.Inches ? ToCentimeters(height) : height,
                    weightUnit == WeightUnit.Pounds ? ToKgs(weight) : weight,
                    age,
                    user.UserStats.Gender,
                    user.Goal.ActivityLevel,
                    weeklyGoal);

                var updatedUser = await _userRepository.UpdateCurrentWeightAsync(user, request.Measurement, weeklyGoal, calories);
                var lastLog = updatedUser.ProgressLogs
                    .Where(x => x.MeasurementType == MeasurementType.Weight)
                    .OrderBy(x => x.LoggedOn)
                    .LastOrDefault();

                return lastLog is null
                    ? Result.CreateWithError<ProgressLogResponse>(EvaluationTypes.Failed, "Error occured.")
                    : Result.Create(lastLog.ToResponse());
            }
        
            var newLog = new ProgressLog
            {
                Measurement = request.Measurement,
                MeasurementType = measurementType,
                LoggedOn = request.Date.Date.Add(DateTime.Now.TimeOfDay),
                UserId = currentUserId.Value
            };

            await _progressLogRepository.SaveEntityAsync(newLog);

            return Result.Create(newLog.ToResponse());
        }

        public async Task<Result<Int32>> DeleteProgressLogAsync(Int32 progressLogId)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<Int32>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var progressLog = await _progressLogRepository.GetProgressLogAsync(currentUserId.Value, progressLogId);

            var result = await _progressLogRepository.DeleteProgressLog(progressLog);

            return result <= 0
                ? Result.CreateWithError<Int32>(EvaluationTypes.Failed, "Cound not delete progress log.")
                : Result.Create(progressLogId);
        }

    }
}
