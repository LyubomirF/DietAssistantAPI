using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Business.Contracts.Models.FoodLog.Requests;
using DietAssistant.Business.Contracts.Models.FoodLog.Responses;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;

namespace DietAssistant.Business
{
    public class FoodLogService : IFoodLogService
    {
        private readonly IFoodCatalogService _foodCatalogService;
        private readonly IMealRepository _mealRepository;

        public FoodLogService(
            IFoodCatalogService foodCatalogService,
            IMealRepository mealRepository)
        {
            _foodCatalogService = foodCatalogService;
            _mealRepository = mealRepository;
        }

        public async Task<Result<NewMealLogResponse>> LogNewMealAsync(LogNewMealRequest request)
        {
            var food = (await _foodCatalogService
                .GetFoodsAsync(request.FoodServings.Select(x => x.FoodId))).Data;

            if (food is null)
                return Result
                    .CreateWithError<NewMealLogResponse>(EvaluationTypes.NotFound, "Food with id not found.");

            var lastMeal = await _mealRepository.GetLastMealAsync(request.Date);
            var foodServings = request.FoodServings
                    .Select(x => new FoodServing
                    {
                        FoodId = x.FoodId,
                        ServingSize = x.Size,
                        ServingUnit = x.Unit,
                        NumberOfServings = x.Number
                    }).ToList();

            var newMeal = new Meal();

            if (lastMeal is null)
            {
                newMeal.EatenOn = request.Date;
                newMeal.Order = 1;
                newMeal.FoodServings = foodServings;
            }
            else
            {
                newMeal.EatenOn = request.Date;
                newMeal.Order = lastMeal.Order + 1;
                newMeal.FoodServings = foodServings;
            }

            await _mealRepository.SaveEntityAsync(newMeal);

            return Result.Create(GetNewMealLogResponse(
                foodServings,
                food.ToList(),
                newMeal.Order,
                request.Date));
        }

        public async Task<Result<FoodLogResponse>> LogFoodAsync(LogRequest request)
        {
            var food = (await _foodCatalogService.GetFoodByIdAsync(request.FoodId)).Data;

            if (food is null)
                return Result
                    .CreateWithError<FoodLogResponse>(EvaluationTypes.NotFound, "Food with id not found.");

            var foodServing = new FoodServing
            {
                FoodId = request.FoodId,
                NumberOfServings = request.NumberOfServings,
                ServingSize = request.ServingSizeAmount,
                ServingUnit = request.ServingSizeUnit
            };

            var meal = await _mealRepository.GetByIdAsync(request.MealId);

            if (meal is null)
                return Result
                    .CreateWithError<FoodLogResponse>(EvaluationTypes.NotFound, "Meal not found.");

            meal.FoodServings.Add(foodServing);

            await _mealRepository.SaveEntityAsync(meal);

            return Result.Create(GetFoodLogResponse(meal, food, foodServing));
        }

        private FoodLogResponse GetFoodLogResponse(Meal meal, FoodDetails food, FoodServing foodServing)
        {
            return new FoodLogResponse
            {
                MealId = meal.MealId,
                MealOrder = meal.Order,
                Food = new LoggedFood
                {
                    FoodId = food.FoodId,
                    FoodName = food.FoodName,
                    Nutrition = new LoggedNutrition
                    {
                        Calories = CalculateNutrientTotal("Calories", food, foodServing),
                        Carbs = CalculateNutrientTotal("Carbs", food, foodServing),
                        Fat = CalculateNutrientTotal("Fat", food, foodServing),
                        Protein = CalculateNutrientTotal("Protein", food, foodServing),
                    }
                }
            };
        }
        private NewMealLogResponse GetNewMealLogResponse(
            IEnumerable<FoodServing> foodServings,
            List<FoodDetails> foods,
            Int32 mealNumber,
            DateTime date)
        {
            return new NewMealLogResponse
            {
                EatenOn = date,
                MealNumber = mealNumber,
                Foods = foodServings
                .Select(fs => new
                {
                    FoodServing = fs,
                    Food = foods.SingleOrDefault(x => x.FoodId == fs.FoodId)
                })
                .Select(x => new LoggedFood
                {
                    FoodId = x.Food.FoodId,
                    FoodName = x.Food.FoodName,
                    Nutrition = new Contracts.Models.FoodLog.Responses.LoggedNutrition
                    {
                        Carbs = CalculateNutrientTotal("Carbohydrates", x.Food, x.FoodServing),
                        Fat = CalculateNutrientTotal("Fat", x.Food, x.FoodServing),
                        Protein = CalculateNutrientTotal("Protein", x.Food, x.FoodServing),
                        Calories = CalculateNutrientTotal("Calories", x.Food, x.FoodServing),
                    }
                })
                .ToList(),
            };
        }

        public Double CalculateNutrientTotal(string name, FoodDetails food, FoodServing foodServing)
            => food.Nutrition.Nutrients.Single(x => x.Name == name).Amount
                * foodServing.ServingSize
                * foodServing.NumberOfServings;
    }

}
