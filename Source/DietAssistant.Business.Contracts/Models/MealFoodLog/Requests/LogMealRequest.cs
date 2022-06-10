using DietAssistant.Business.Contracts.Models.FoodServing;

namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Requests
{
    public class LogMealRequest
    {
        public DateTime Date { get; set; }

        public List<FoodServingDto> FoodServings { get; set; }
    }
}
