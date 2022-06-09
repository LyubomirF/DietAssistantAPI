using DietAssistant.Business.Contracts.Models.FoodLog.Requests;

namespace DietAssistant.Business.Contracts
{
    public interface IFoodLogService
    {
        Task LogFood(LogRequest request);
    }
}
