using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.Domain.Enums;
using Microsoft.EntityFrameworkCore;

#pragma warning disable

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

            query = query.OrderByDescending(x => x.LoggedOn);

            var totalCount = query.Count();

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            return (result, totalCount);
        }

        public Task<ProgressLog> GetProgressLogAsync(int userId, int progressLogId)
            => _dbContext.ProgressLogs
                .SingleOrDefaultAsync(x => x.UserId == userId && x.ProgressLogId == progressLogId);

        public Task<Int32> DeleteProgressLog(ProgressLog progressLog)
            => DeleteAsync(progressLog);
        
        Task<ProgressLog> IRepository<ProgressLog>.GetByIdAsync(Int32 id)
            => GetByIdAsync(id);
    }
}
