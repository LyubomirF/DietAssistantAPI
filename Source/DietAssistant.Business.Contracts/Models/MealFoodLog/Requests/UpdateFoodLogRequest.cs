using DietAssistant.Business.Contracts.Models.FoodServing;

namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Requests
{
    public class UpdateFoodLogRequest
    {
        public FoodServingDto FoodServing { get; set; }
    }
}
