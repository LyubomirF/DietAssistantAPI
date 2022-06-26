namespace DietAssistant.Business.Contracts.Models.DietPlanning.Responses
{
    public class SimpleMealPlanResponse
    {
        public Int32 MealPlanId { get; set; }

        public String MealName { get; set; }

        public String Time { get; set; }

        public List<FoodPlanResponse> FoodPlan { get; set; }
    }
}
