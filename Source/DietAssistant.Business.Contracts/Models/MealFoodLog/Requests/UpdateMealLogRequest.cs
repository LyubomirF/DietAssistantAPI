using DietAssistant.Business.Contracts.Models.FoodServing;

namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Requests
{
    public class UpdateMealLogRequest
    {
        public List<FoodServingDto> FoodServings { get; set; }
    }
}
