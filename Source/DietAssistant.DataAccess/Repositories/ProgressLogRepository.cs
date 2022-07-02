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

        public async Task<(IEnumerable<ProgressLog> ProgressLogs, Int32 TotalCount)> GetProgressLogPagedAsync(
            Int32 userId,
            MeasurementType measurementType,
            DateTime? periodStart,
            DateTime? periodEnd,
            Int32 page,
            Int32 pageSize)
        {
            var query = _dbContext.ProgressLogs
                .Where(x => x.UserId == userId && x.MeasurementType == measurementType);

            if (periodStart.HasValue)
                query = query.Where(x => x.LoggedOn >= periodStart.Value);

            if (periodEnd.HasValue)
                query = query.Where(x => x.LoggedOn <= periodEnd.Value);

            var totalCount = query.Count();

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            return (result, totalCount);
        }

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
