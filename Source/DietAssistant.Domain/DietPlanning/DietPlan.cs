namespace DietAssistant.Domain.DietPlanning
{
    public class DietPlan
    {
        public Int32 DietPlanId { get; set; }

        public string PlanName { get; set; }

        public User User { get; set; }

        public Int32 UserId { get; set; }

        public ICollection<DayPlan> DayPlans { get; set; }
    }
}
