using DietAssistant.Domain;
using DietAssistant.Domain.Enums;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IProgressLogRepository : IRepository<ProgressLog>
    {
        Task<(IEnumerable<ProgressLog> ProgressLogs, Int32 TotalCount)> GetProgressLogPagedAsync(
            Int32 userId,
            MeasurementType measurementType,
            DateTime? periodStart,
            DateTime? periodEnd,
            Int32 page,
            Int32 pageSize);

        Task<ProgressLog> GetProgressLogAsync(Int32 userId, Int32 progressLogId);

        Task<Int32> DeleteProgressLog(ProgressLog progressLog);
    }
}
