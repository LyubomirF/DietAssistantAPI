using DietAssistant.Domain;

namespace DietAssistant.DataAccess.Contracts
{
    public interface INutritionGoalRepository : IRepository<NutritionGoal>
    {
       /// Task<NutritionGoal> GetLatestNutritionGoalByUserId(Int32 userId);
    }
}
