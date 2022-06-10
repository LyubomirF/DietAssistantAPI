namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Responses
{
    public class MealLogResponse
    {
        public DateTime EatenOn { get; set; }

        public int MealNumber { get; set; }

        public List<LoggedFood> Foods { get; set; }

        public double TotalCalories { get; set; }

        public double TotalCarbs { get; set; }

        public double TotalFat { get; set; }

        public double TotalProtein { get; set; }
    }
}
