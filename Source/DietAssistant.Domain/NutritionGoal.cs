namespace DietAssistant.Domain
{
    public class NutritionGoal
    {
        public Int32 NutritionGoalId { get; set; }

        public Double Calories { get; set; }

        public Double PercentProtein { get; set; }

        public Double PercentCarbs { get; set; }

        public Double PercentFat { get; set; }

        public DateTime ChangedOnUTC { get; set; }
    }
}
