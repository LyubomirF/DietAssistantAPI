using DietAssistant.Domain;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IGoalRespository : IRepository<Goal>
    {
        Task<Goal> GetGoalByUserId(Int32 userId);
    }
}
