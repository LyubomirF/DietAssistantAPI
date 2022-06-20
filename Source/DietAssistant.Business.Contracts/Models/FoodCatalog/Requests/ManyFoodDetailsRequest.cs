namespace DietAssistant.Business.Contracts.Models.FoodCatalog.Requests
{
    public class ManyFoodDetailsRequest
    {
        public String FoodId { get; set; }

        public ServingRequest Serving { get; set; }
    }
}
