namespace DietAssistant.Domain.DietPlanning
{
    public class FoodPlan
    {
        public Int32 FoodPlanId { get; set; }

        public String FoodId { get; set; }  

        public Double ServingSize { get; set; }

        public String Unit { get; set; }
    }
}