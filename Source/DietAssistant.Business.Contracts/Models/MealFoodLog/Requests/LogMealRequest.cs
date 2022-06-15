using DietAssistant.Business.Contracts.Models.FoodServing.Requests;

namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Requests
{
    public class LogMealRequest
    {
        public DateTime Date { get; set; }

        public List<LogUpdateFoodServingRequest> FoodServings { get; set; }
    }
}
