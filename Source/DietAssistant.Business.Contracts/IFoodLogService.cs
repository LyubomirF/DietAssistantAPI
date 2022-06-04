using DietAssistant.Business.Contracts.Models.Requests;

namespace DietAssistant.Business.Contracts
{
    public interface IFoodLogService
    {
        Task LogFood(LogRequest request);
    }
}
