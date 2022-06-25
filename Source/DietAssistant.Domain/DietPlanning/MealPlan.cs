namespace DietAssistant.Domain.DietPlanning
{
    public class MealPlan
    {
        public Int32 MealPlanId { get; set; }

        public String MealPlanName { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan Time { get; set; }

        public ICollection<FoodPlan> FoodPlans { get; set; }
    }
}
