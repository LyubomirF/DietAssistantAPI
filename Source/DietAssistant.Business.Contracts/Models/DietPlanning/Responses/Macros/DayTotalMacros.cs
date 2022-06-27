namespace DietAssistant.Business.Contracts.Models.DietPlanning.Responses.Macros
{
    public class DayTotalMacros
    {
        public DayOfWeek Day { get; set; }

        public List<MealPlanTotalMacrosDto> MealPlansMacros { get; set; }
    }
}
