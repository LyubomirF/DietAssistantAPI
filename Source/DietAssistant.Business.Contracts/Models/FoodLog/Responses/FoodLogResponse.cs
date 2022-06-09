namespace DietAssistant.Business.Contracts.Models.FoodLog.Responses
{
    public class FoodLogResponse
    {
        public Int32 MealId { get; set; }   

        public Int32 MealOrder { get; set; }

        public DateTime EatenOn { get; set; }

        public LoggedFood Food { get; set; }
    }
}
