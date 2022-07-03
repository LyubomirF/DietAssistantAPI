namespace DietAssistant.Business.Contracts.Models.Goal.Requests
{
    public class ChangeCurrentWeighRequest
    {
        public Double CurrentWeight { get; set; }
    }

    public class ChangeGoalWeightRequest
    {
        public Double GoalWeight { get; set; }
    }

    public class ChangeWeeklyGoalRequest
    {
        public String WeeklyGoal { get; set; }
    }

    public class ChangeActivityLevelRequest
    {
        public String ActivityLevel { get; set; }
    }
}
