using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Paging;
using DietAssistant.Business.Contracts.Models.ProgressLog.Requests;
using DietAssistant.Business.Contracts.Models.ProgressLog.Responses;
using DietAssistant.Business.Mappers;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.Domain.Enums;

namespace DietAssistant.Business
{
    public class ProgressLogService : IProgressLogService
    {
        private readonly IUserResolverService _userResolverService;
        private readonly IProgressLogRepository _progressLogRepository;

        public ProgressLogService(
            IUserResolverService userResolverService,
            IProgressLogRepository progressLogRepository)
        {
            _progressLogRepository = progressLogRepository;
            _userResolverService = userResolverService;
        }

        public async Task<Result<PagedResult<ProgressLogResponse>>> GetProgressLogsPagedAsync(ProgressLogFilterRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<PagedResult<ProgressLogResponse>>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            if(!Enum.TryParse(request.MeasurementType, out MeasurementType measurementType))
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

            var newLog = new ProgressLog 
            {
                Measurement = request.Measurement,
                MeasurementType = measurementType,
                LoggedOn =request.Date,
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
