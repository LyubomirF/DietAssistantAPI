using DietAssistant.Business.Contracts.Models.Goal.Responses;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IGoalService
    {
        Task<Result<GoalResponse>> GetGoalAsync();
    }
}
