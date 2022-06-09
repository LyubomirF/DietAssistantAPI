using DietAssistant.Business.Contracts.Models.FoodServing;

namespace DietAssistant.Business.Contracts.Models.FoodLog.Requests
{
    public class LogNewMealRequest
    {
        public DateTime Date { get; set; }

        public List<FoodServingDto> Serving { get; set; }
    }
}
