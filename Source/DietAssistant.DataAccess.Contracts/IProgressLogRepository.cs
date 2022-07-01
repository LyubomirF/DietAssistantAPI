using DietAssistant.Domain;
using DietAssistant.Domain.Enums;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IProgressLogRepository : IRepository<ProgressLog>
    {
        Task<IEnumerable<ProgressLog>> GetProgressLogsAsync(Int32 userId, MeasurementType type);

        Task UpdateRangeAsync(IEnumerable<ProgressLog> progressLogs);
    }
}
