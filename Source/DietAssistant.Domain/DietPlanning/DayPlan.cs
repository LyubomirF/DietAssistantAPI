namespace DietAssistant.Domain.DietPlanning
{
    public class DayPlan
    {
        public Int32 DayPlanId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public ICollection<MealPlan> MealPlans { get; set; }
    }
}
