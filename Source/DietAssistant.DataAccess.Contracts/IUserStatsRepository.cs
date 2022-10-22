using DietAssistant.Domain;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IUserStatsRepository : IRepository<UserStats>
    {
        Task<UserStats> GetUserStatsAsync(Int32 userId);
    } 
}
