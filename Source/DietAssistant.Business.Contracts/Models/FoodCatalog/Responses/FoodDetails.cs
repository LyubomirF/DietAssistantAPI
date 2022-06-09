#pragma warning disable CS8618

namespace DietAssistant.Business.Contracts.Models.FoodCatalog.Responses
{
    public class FoodDetails
    {
        public int FoodId { get; set; }

        public string FoodName { get; set; }

        public Nutrition Nutrition { get; set; }

        public Serving ServingInformation { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }
    }
}
