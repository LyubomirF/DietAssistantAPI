namespace DietAssistant.Business.Contracts.Models.DietPlanning.Responses.Macros
{
    public class DayMacros
    {
        public String Day { get; set; }

        public List<MealPlanMacros> MealPlansMacros { get; set; }

        public Double TotalCalories { get; set; }

        public Double TotalProtein { get; set; }

        public Double TotalCarbs { get; set; }

        public Double TotalFat { get; set; }
    }
}
