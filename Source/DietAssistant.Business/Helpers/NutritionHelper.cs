using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Domain;

namespace DietAssistant.Business.Helpers
{
    public static class NutritionHelper
    {
        public static Double CalculateNutrientTotal(this FoodDetails food, FoodServing foodServing, string nutrientName)
        {
            var servingSize = foodServing.ServingSize;
            var numberOfServings = foodServing.NumberOfServings;

            var foodDefaultServing = food.ServingInformation.Size;

            var nutrientAmountPerServing = food.Nutrition.Nutrients.Single(x => x.Name == nutrientName).Amount;

            var ratio = (servingSize * numberOfServings) / foodDefaultServing;

            return Math.Round(ratio * nutrientAmountPerServing, 2);
        }
    }
}
