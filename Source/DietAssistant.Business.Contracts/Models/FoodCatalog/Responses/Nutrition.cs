#pragma warning disable CS8618

namespace DietAssistant.Business.Contracts.Models.FoodCatalog.Responses
{
    public class Nutrition
    {
        public List<Nutrient> Nutrients { get; set; }

        public Double GetNutrientAmount(String nutrientName)
        {
            if (Nutrients is null || !Nutrients.Any())
                return 0;

            var nutrientInfo = Nutrients.SingleOrDefault(x => x.Name == nutrientName);

            if (nutrientInfo is null)
                return 0;

            return nutrientInfo.Amount;
        }

        public Double CalculateNutrientAmount(String nutrientName, Double numberOfServings)
        {
            if (Nutrients is null || !Nutrients.Any())
                return 0;

            var nutrientInfo = Nutrients.SingleOrDefault(x => x.Name == nutrientName);

            if (nutrientInfo is null)
                return 0;

            return nutrientInfo.Amount * numberOfServings;
        }
    }
}
