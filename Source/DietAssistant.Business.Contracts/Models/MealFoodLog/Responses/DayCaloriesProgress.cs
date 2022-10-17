namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Responses
{
    public class DayCaloriesProgress
    {
        public IEnumerable<MealCalories> MealCalories { get; set; }

        public Double GoalCalories { get; set; }

        public Double LoggedCalories { get; set; }
    }
}
