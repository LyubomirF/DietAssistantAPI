using DietAssistant.Business.Contracts.Models.Paging;
using DietAssistant.Business.Contracts.Models.ProgressLog.Responses;
using DietAssistant.Domain;

namespace DietAssistant.Business.Mappers
{
    internal static class ProgressLogMapper
    {
        public static PagedResult<ProgressLogResponse> ToPagedResult(
            this IEnumerable<ProgressLog> progressLogs,
            Int32 page,
            Int32 pageSize,
            Int32 totalCount)
        {
            return new PagedResult<ProgressLogResponse>
            {
                Results = progressLogs.Select(x => new ProgressLogResponse
                {
                    Measurement = x.Measurement,
                    MeasurementType = x.MeasurementType.ToString(),
                    LoggedOn = x.LoggedOn
                })
                .ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
