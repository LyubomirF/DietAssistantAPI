namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Responses
{
    public class LoggedFood
    {
        public Int32 FoodServingId { get; set; }

        public Int32 FoodId { get; set; }

        public String FoodName { get; set; }

        public LoggedNutrition Nutrition { get; set; }

    }
}
