using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.Domain.Enums;
using Microsoft.EntityFrameworkCore;

#pragma warning disable

namespace DietAssistant.DataAccess.Repositories
{
    public class UserStatsRepository : Repository<UserStats>, IUserStatsRepository
    {
        public UserStatsRepository(DietAssistantDbContext dbContext)
            : base(dbContext) { }

        public Task<UserStats> GetUserStatsAsync(Int32 userId)
           => _dbContext.UsersStats
                 .SingleOrDefaultAsync(x => x.UserId == userId);

        Task<UserStats> IRepository<UserStats>.GetByIdAsync(int id)
            => GetByIdAsync(id);

    }
}
