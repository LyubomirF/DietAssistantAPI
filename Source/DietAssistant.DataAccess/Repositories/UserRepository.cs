using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8619
#pragma warning disable CS8603

namespace DietAssistant.DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DietAssistantDbContext dbContext)
            : base(dbContext) { }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (email is null)
                return null;

            return await _dbContext.Users
               .SingleOrDefaultAsync(x => x.Email == email.Trim());
        }

        Task<User> IRepository<User>.GetByIdAsync(int id)
            => GetByIdAsync(id);
    }
}
