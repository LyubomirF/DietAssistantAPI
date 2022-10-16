using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Business.Contracts.Models.FoodServing.Responses;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Responses;
using DietAssistant.Common;
using DietAssistant.Domain;

namespace DietAssistant.Business.Mappers
{
    internal static class MealMapper
    {
        public static MealLogResponse ToResponse(
            this Meal meal,
            IEnumerable<FoodServing> foodServings,
            IReadOnlyCollection<FoodDetails> foods)
        {
            var loggedFood = foodServings
                .Select(fs => new
                {
                    FoodServing = fs,
                    Food = foods.SingleOrDefault(x => x.FoodId == fs.FoodId)
                })
                .Select(x => new LoggedFoodServing
                {
                    FoodServingId = x.FoodServing.FoodServingId,
                    FoodId = x.Food.FoodId,
                    FoodName = x.Food.FoodName,
                    Nutrition = new LoggedNutrition
                    {
                        Carbs = x.Food.Nutrition.CalculateNutrientAmount(
                            DietAssistantConstants.Carbohydrates,
                            x.FoodServing.NumberOfServings),
                        Fat = x.Food.Nutrition.CalculateNutrientAmount(
                            DietAssistantConstants.Fat,
                            x.FoodServing.NumberOfServings),
                        Protein = x.Food.Nutrition.CalculateNutrientAmount(
                            DietAssistantConstants.Protein,
                            x.FoodServing.NumberOfServings),
                        Calories = x.Food.Nutrition.CalculateNutrientAmount(
                            DietAssistantConstants.Calories,
                            x.FoodServing.NumberOfServings),
                    }
                })
                .ToList();

            return new MealLogResponse
            {
                MealId = meal.MealId,
                EatenOn = meal.EatenOn,
                MealNumber = meal.Order,
                Foods = loggedFood,
                TotalCalories = loggedFood
                    .Select(x => x.Nutrition.Calories)
                    .Aggregate((x, y) => x + y),
                TotalCarbs = loggedFood
                    .Select(x => x.Nutrition.Carbs)
                    .Aggregate((x, y) => x + y),
                TotalFat = loggedFood
                    .Select(x => x.Nutrition.Fat)
                    .Aggregate((x, y) => x + y),
                TotalProtein = loggedFood
                    .Select(x => x.Nutrition.Protein)
                    .Aggregate((x, y) => x + y),
            };
        }
    }
}
