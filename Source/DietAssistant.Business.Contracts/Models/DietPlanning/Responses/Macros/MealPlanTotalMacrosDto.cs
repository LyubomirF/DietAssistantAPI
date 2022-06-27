namespace DietAssistant.Business.Contracts.Models.DietPlanning.Responses.Macros
{
    public class MealPlanTotalMacrosDto
    {
        public Int32 MealPlanId { get; set; }

        public String MealPlanName { get; set; }

        public Double TotalCalories { get; set; }

        public Double TotalProtein { get; set; }

        public Double TotalCarbs { get; set; }

        public Double TotalFat { get; set; }
    }
}
