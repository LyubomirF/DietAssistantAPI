#pragma warning disable CS8618

namespace DietAssistant.Business.Contracts.Models.FoodCatalog.Responses
{
    public class FoodDetails
    {
        public String FoodId { get; set; }

        public String FoodName { get; set; }

        public Nutrition Nutrition { get; set; }

        public Serving ServingInformation { get; set; }

        public List<String> PossibleUnits { get; set; }

        public String ImagePath { get; set; }
    }
}
