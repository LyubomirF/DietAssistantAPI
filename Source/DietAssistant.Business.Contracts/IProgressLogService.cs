using DietAssistant.Business.Contracts.Models.Paging;
using DietAssistant.Business.Contracts.Models.ProgressLog.Requests;
using DietAssistant.Business.Contracts.Models.ProgressLog.Responses;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IProgressLogService
    {
        Task<Result<PagedResult<ProgressLogResponse>>> GetProgressLogsPagedAsync(ProgressLogFilterRequest request);

        Task<Result<ProgressLogResponse>> AddProgressLogAsync(AddProgressLogRequest request);

        Task<Result<Int32>> DeleteProgressLogAsync(Int32 progressLogId);
    }
}
