namespace DietAssistant.Business.Contracts.Models.DietPlanning.Responses
{
    public class MealPlanResponse
    {
        public Int32 DietPlanId { get; set; }   

        public Int32 MealPlanId { get; set; }

        public String DayOfWeek { get; set; }

        public String Time { get; set; }

        public List<FoodPlanResponse> FoodPlan { get; set; }
    }
}
