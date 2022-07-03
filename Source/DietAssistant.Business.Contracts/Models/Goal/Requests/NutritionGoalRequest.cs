namespace DietAssistant.Business.Contracts.Models.Goal.Requests
{
    public class NutritionGoalRequest
    {
        public Double Calories { get; set; }

        public Double PercentProtein { get; set; }

        public Double PercentCarbs { get; set; }

        public Double PercentFat { get; set; }
    }
}
