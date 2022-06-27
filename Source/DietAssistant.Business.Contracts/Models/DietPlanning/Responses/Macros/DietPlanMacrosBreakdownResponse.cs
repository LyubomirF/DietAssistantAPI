namespace DietAssistant.Business.Contracts.Models.DietPlanning.Responses.Macros
{
    public class DietPlanMacrosBreakdownResponse
    {
        public Int32 DietPlanId { get; set; }

        public String DietPlanName { get; set; }

        public List<DayMacros> DaysMacros { get; set; }
    }
}
