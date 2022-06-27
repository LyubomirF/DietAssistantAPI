namespace DietAssistant.Business.Contracts.Models.DietPlanning.Responses.Macros
{
    public class MealPlanMacros
    {
        public Int32 MealPlanId { get; set; }

        public String MealPlanName { get; set; }

        public Double PercentageOfTotalCalories { get; set; }

        public Double PercentageOfTotalProtein { get; set; }

        public Double PercentageOfTotalCarbs { get; set; }

        public Double PercantageOfTotalFat { get; set; }
    }
}
