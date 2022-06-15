using DietAssistant.Business.Contracts.Models.FoodServing.Requests;

namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Requests
{
    public class UpdateMealLogRequest
    {
        public List<LogUpdateFoodServingRequest> FoodServings { get; set; }
    }
}
