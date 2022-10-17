namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Responses
{
    public class MealCalories
    {
        public Int32 MealId { get; set; }

        public Int32 MealNumber { get; set; }

        public Double Calories { get; set; }

        public Double PercentOfTotal { get; set; }
    }
}
