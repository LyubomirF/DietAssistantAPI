namespace DietAssistant.Business.Contracts.Models.DietPlanning.Requests
{
    public class FoodPlanRequest
    {
        public String FoodId { get; set; }

        public Double ServingSize { get; set; }

        public String Unit { get; set; }
    }
}
