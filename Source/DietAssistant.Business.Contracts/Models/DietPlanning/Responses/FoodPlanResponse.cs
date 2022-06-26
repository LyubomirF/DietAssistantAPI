namespace DietAssistant.Business.Contracts.Models.DietPlanning.Responses
{
    public class FoodPlanResponse
    {
        public Int32 FoodPlanId { get; set; }

        public String FoodId { get; set; }

        public String FoodName { get; set; }

        public Double ServingSize { get; set; }

        public String Unit { get; set; }    
    }
}
