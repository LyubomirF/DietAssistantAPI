using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DietAssistant.DataAccess.Repositories
{
    public class ProgressLogRepository : Repository<ProgressLog>, IProgressLogRepository
    {
        public ProgressLogRepository(DietAssistantDbContext dbContext) 
            : base(dbContext) { }

        public async Task<IEnumerable<ProgressLog>> GetProgressLogsAsync(Int32 userId, MeasurementType type)
            => await _dbContext.ProgressLogs
                .Where(x => x.UserId == userId && x.MeasurementType == type)
                .ToListAsync();

        public async Task UpdateRangeAsync(IEnumerable<ProgressLog> progressLogs)
        {
            _dbContext.ProgressLogs.UpdateRange(progressLogs);

            await _dbContext.SaveChangesAsync();
        }

        Task<ProgressLog> IRepository<ProgressLog>.GetByIdAsync(Int32 id)
            => GetByIdAsync(id);
    }
}
