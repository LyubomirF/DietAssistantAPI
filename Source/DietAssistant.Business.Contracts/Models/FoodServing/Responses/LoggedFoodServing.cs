namespace DietAssistant.Business.Contracts.Models.FoodServing.Responses
{
    public class LoggedFoodServing
    {
        public int FoodServingId { get; set; }

        public int FoodId { get; set; }

        public string FoodName { get; set; }

        public LoggedNutrition Nutrition { get; set; }

    }
}
