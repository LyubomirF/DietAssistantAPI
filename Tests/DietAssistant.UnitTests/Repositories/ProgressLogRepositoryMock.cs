using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.Domain.Enums;
using DietAssistant.UnitTests.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable 

namespace DietAssistant.UnitTests.Repositories
{
    using static DatabaseMock;

    internal class ProgressLogRepositoryMock : IRepository<ProgressLog>, IProgressLogRepository
    {
        public Task<ProgressLog> GetProgressLogAsync(int userId, int progressLogId)
        {
            var user = Users.SingleOrDefault(x => x.UserId == userId);

            var progressLog = user?.ProgressLogs.SingleOrDefault(x => x.ProgressLogId == progressLogId);

            return Task.FromResult(progressLog);
        }

        public async Task<(IEnumerable<ProgressLog> ProgressLogs, int TotalCount)> GetProgressLogPagedAsync(int userId, MeasurementType measurementType, DateTime? periodStart, DateTime? periodEnd, int page, int pageSize)
        {
            var user = Users.SingleOrDefault(x => x.UserId == userId);

            var progressLogs = user?.ProgressLogs.Where(x => x.MeasurementType == measurementType);

            if (periodStart.HasValue)
                progressLogs = progressLogs.Where(x => x.LoggedOn >= periodStart.Value);

            if (periodEnd.HasValue)
                progressLogs = progressLogs.Where(x => x.LoggedOn <= periodEnd.Value);

            var totalCount = progressLogs.Count();

            var result = progressLogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return await Task.FromResult((result, totalCount));
        }

        public Task<ProgressLog> GetByIdAsync(int id)
            => Task.FromResult(Users
                .SelectMany(x => x.ProgressLogs)
                .SingleOrDefault(x => x.ProgressLogId == id));
        
        public Task<int> DeleteProgressLog(ProgressLog progressLog)
        {
            var user = Users.SingleOrDefault(x => x.UserId == progressLog.UserId);

            user.ProgressLogs.Remove(progressLog);

            return Task.FromResult(1);
        }

        public Task SaveEntityAsync(ProgressLog entity)
        {
            var user = Users.SingleOrDefault(x => x.UserId == entity.UserId);
            user.ProgressLogs.Add(entity);

            return Task.FromResult(1);
        }
    }
}
