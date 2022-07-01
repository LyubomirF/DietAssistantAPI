using DietAssistant.Domain;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IProgressLogRepository : IRepository<ProgressLog>
    {
        Task<IEnumerable<ProgressLog>> GetProgressLogsAsync(Int32 userId);

        Task UpdateRangeAsync(IEnumerable<ProgressLog> progressLogs);
    }
}
