namespace DietAssistant.Business.Contracts.Models.DietPlanning.Requests
{
    public class AddUpdateMealRequest
    {
        public String MealName { get; set; }

        public DayOfWeek Day { get; set; }

        public TimeSpan Time { get; set; }

        public List<FoodPlanRequest> FoodPlanRequests { get; set;}
    }
}
