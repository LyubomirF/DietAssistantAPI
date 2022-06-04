namespace DietAssistant.Business.Contracts.Models.Requests
{
    public class LogRequest
    {
        public Int32 FoodId { get; set; }

        public Int32 MealId { get; set; }

        public Double ServingSizeAmount { get; set; }

        public String ServingSizeUnit { get; set; }

        public Int32 NumberOfServings { get; set; }
    }
}
