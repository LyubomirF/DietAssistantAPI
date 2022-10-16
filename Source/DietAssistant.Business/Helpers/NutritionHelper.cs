using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;

namespace DietAssistant.Business.Helpers
{
    public static class NutritionHelper
    {
        private const Double GramOunceMulpilier = 28.35;

        public static FoodDetails CalculateNutrition(
            this FoodDetails food,
            Double defaultServingSize,
            String defaultUnit,
            Double targetAmount,
            String targetUnit)
        {
            var convertedTargetAmount = targetAmount;

            if (defaultUnit == "g" && targetUnit == "oz")
            {
                convertedTargetAmount = targetAmount * GramOunceMulpilier;
            }

            if (defaultUnit == "oz" && targetUnit == "g")
            {
                convertedTargetAmount = targetAmount / GramOunceMulpilier;
            }

            foreach (var nutrient in food.Nutrition.Nutrients)
            {
                nutrient.Amount = CalculateNutrientAmount(defaultServingSize, nutrient.Amount, convertedTargetAmount);
            }

            food.ServingInformation = new Serving
            {
                Number = 1,
                Size = targetAmount,
                Unit = targetUnit
            };

            return food;
        }

        public static Double CalculateNutrientAmount(
            Double defautServingSize,
            Double defaultNutrientAmount,
            Double targetFoodAmount)
        {
            var ratio = targetFoodAmount / defautServingSize;

            return Math.Round(ratio * defaultNutrientAmount, 2);
        }
    }
}
