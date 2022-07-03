namespace DietAssistant.Business.Contracts.Models.Goal.Responses
{
    public class GoalResponse
    {
        public Double StartWeight { get; set; }

        public DateTime StartDate { get; set; }

        public Double CurrentWeight { get; set; }

        public Double GoalWeight { get; set; }

        public String WeeklyGoal { get; set; }

        public String ActivityLevel { get; set; }

        public NutritionGoal NutritionGoal { get; set; }
    }
}
