namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Responses
{
    public class DayMacrosProgress
    {
        public Double CarbsLogged { get; set; }
        public Double ProteinLogged { get; set; }
        public Double FatsLogged { get; set; }

        public Double CarbsPercentage { get; set; }
        public Double ProteinPercentage { get; set; }
        public Double FatsPercentage { get; set; }

        public Double CarbsGoal { get; set; }
        public Double ProteinGoal { get; set; }
        public Double FatsGoal { get; set; }

        public Double CarbsGoalPercentage { get; set; }
        public Double ProteinGoalPercentage { get; set; }
        public Double FatsGoalPercentage { get; set; }

    }
}
