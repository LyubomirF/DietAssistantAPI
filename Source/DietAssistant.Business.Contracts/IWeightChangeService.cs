using DietAssistant.Domain;

namespace DietAssistant.Business.Contracts
{
    public interface IWeightChangeService
    {
        Task HandleWeightChange(Int32 userId, Double weight, Goal goal, UserStats userStats);

        Task HandleGoalWeightChange(Int32 userId, Double goalWeight, Goal goal, UserStats userStats);
    }
}
