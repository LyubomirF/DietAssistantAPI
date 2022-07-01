using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using Microsoft.EntityFrameworkCore;

namespace DietAssistant.DataAccess.Repositories
{
    public class ProgressLogRepository : Repository<ProgressLog>, IProgressLogRepository
    {
        public ProgressLogRepository(DietAssistantDbContext dbContext) 
            : base(dbContext) { }

        public async Task<IEnumerable<ProgressLog>> GetProgressLogsAsync(Int32 userId)
            => await _dbContext.ProgressLogs
                .Where(x => x.UserId == userId)
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
