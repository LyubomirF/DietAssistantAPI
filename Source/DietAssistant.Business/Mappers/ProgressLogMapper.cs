using DietAssistant.Business.Contracts.Models.Paging;
using DietAssistant.Business.Contracts.Models.ProgressLog.Responses;
using DietAssistant.Domain;

namespace DietAssistant.Business.Mappers
{
    internal static class ProgressLogMapper
    {
        public static ProgressLogResponse ToResponse(this ProgressLog progressLog)
        {
            return new ProgressLogResponse
            {
                Measurement = progressLog.Measurement,
                MeasurementType = progressLog.MeasurementType.ToString(),
                LoggedOn = progressLog.LoggedOn
            };
        }

        public static PagedResult<ProgressLogResponse> ToPagedResult(
            this IEnumerable<ProgressLog> progressLogs,
            Int32 page,
            Int32 pageSize,
            Int32 totalCount)
        {
            return new PagedResult<ProgressLogResponse>
            {
                Results = progressLogs
                    .Select(x => x.ToResponse())
                    .ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
