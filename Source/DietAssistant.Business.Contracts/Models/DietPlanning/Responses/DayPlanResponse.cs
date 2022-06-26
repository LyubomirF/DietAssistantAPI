namespace DietAssistant.Business.Contracts.Models.DietPlanning.Responses
{
    public class DayPlanResponse
    {
        public String DayOfWeek { get; set; }   

        public List<SimpleMealPlanResponse> Meals { get; set; }
    }
}
