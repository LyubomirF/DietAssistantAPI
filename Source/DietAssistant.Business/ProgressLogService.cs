﻿using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Paging;
using DietAssistant.Business.Contracts.Models.ProgressLog.Requests;
using DietAssistant.Business.Contracts.Models.ProgressLog.Responses;
using DietAssistant.Business.Mappers;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
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

            var (progressLogs, totalCount) = await _progressLogRepository.GetProgressLogPagedAsync(
                currentUserId.Value,
                measurementType,
                request.PeriodStart,
                request.PeriodEnd,
                request.Page,
                request.PageSize);

            return Result.Create(progressLogs.ToPagedResult(request.Page, request.PageSize, totalCount));
        }

    }
}
