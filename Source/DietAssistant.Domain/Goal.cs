using DietAssistant.Domain.Enums;

namespace DietAssistant.Domain
{
    public class Goal
    {
        public Int32 GoalId { get; set; }

        public Double StartWeight { get; set; }

        public Double StartDate { get; set; }

        public Double CurrentWeight { get; set; }

        public Double GoalWeight { get; set; }

        public WeeklyGoal WeeklyGoal { get; set; }

        public NutritionGoal NutritionGoal { get; set; }

        public User User { get; set; }

        public Int32 UserId { get; set; }
    }
}
