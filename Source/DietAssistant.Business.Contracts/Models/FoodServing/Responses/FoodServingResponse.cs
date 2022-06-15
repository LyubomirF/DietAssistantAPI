namespace DietAssistant.Business.Contracts.Models.FoodServing.Responses
{
    public class FoodServingResponse
    {
        public int MealId { get; set; }

        public int MealOrder { get; set; }

        public DateTime EatenOn { get; set; }

        public LoggedFoodServing Food { get; set; }
    }
}
