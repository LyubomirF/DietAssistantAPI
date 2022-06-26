namespace DietAssistant.Business.Contracts.Models.DietPlanning.Responses
{
    public class DietPlanResponse
    {
        public Int32 DietPlanId { get; set; }

        public String Name { get; set; }

        public List<DayPlanResponse> DayPlans { get; set; }
    }
}
