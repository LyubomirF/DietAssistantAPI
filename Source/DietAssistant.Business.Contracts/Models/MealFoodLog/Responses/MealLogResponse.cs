using DietAssistant.Business.Contracts.Models.FoodServing.Responses;

namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Responses
{
    public class MealLogResponse
    {
        public Int32 MealId { get; set; }

        public DateTime EatenOn { get; set; }

        public int MealNumber { get; set; }

        public List<LoggedFoodServing> Foods { get; set; }

        public double TotalCalories { get; set; }

        public double TotalCarbs { get; set; }

        public double TotalFat { get; set; }

        public double TotalProtein { get; set; }
    }
}
