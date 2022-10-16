using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Business.Contracts.Models.FoodServing.Responses;
using DietAssistant.Common;
using DietAssistant.Domain;

namespace DietAssistant.Business.Mappers
{
    internal static class FoodServingMapper
    {
        public static FoodServingResponse ToResponse(
            this FoodServing foodServing,
            Meal meal,
            FoodDetails food)
            => new FoodServingResponse
            {
                MealId = meal.MealId,
                MealOrder = meal.Order,
                EatenOn = meal.EatenOn,
                Food = new LoggedFoodServing
                {
                    FoodServingId = foodServing.FoodServingId,
                    FoodId = food.FoodId,
                    FoodName = food.FoodName,
                    Nutrition = new LoggedNutrition
                    {
                        Calories = food.Nutrition.CalculateNutrientAmount(
                            DietAssistantConstants.Calories,
                            foodServing.NumberOfServings),
                        Carbs = food.Nutrition.CalculateNutrientAmount(
                            DietAssistantConstants.Carbohydrates,
                            foodServing.NumberOfServings),
                        Fat = food.Nutrition.CalculateNutrientAmount(
                            DietAssistantConstants.Fat,
                            foodServing.NumberOfServings),
                        Protein = food.Nutrition.CalculateNutrientAmount(
                            DietAssistantConstants.Protein,
                            foodServing.NumberOfServings)
                    }
                }
            };
    }
}
