namespace DietAssistant.Business.Contracts.Models.FoodServing.Responses
{
    public class LoggedFoodServing
    {
        public int FoodServingId { get; set; }

        public String FoodId { get; set; }

        public String FoodName { get; set; }

        public LoggedNutrition Nutrition { get; set; }

    }
}
