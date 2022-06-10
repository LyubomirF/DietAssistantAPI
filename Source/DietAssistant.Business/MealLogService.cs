using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Responses;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;

namespace DietAssistant.Business
{
    public class MealLogService : IMealLogService
    {
        private readonly IFoodCatalogService _foodCatalogService;
        private readonly IMealRepository _mealRepository;

        public MealLogService(
            IFoodCatalogService foodCatalogService,
            IMealRepository mealRepository)
        {
            _foodCatalogService = foodCatalogService;
            _mealRepository = mealRepository;
        }

        //meals
        public async Task<Result<MealLogResponse>> GetMealById(Int32 id)
        {
            var meal = await _mealRepository.GetMealByIdWithFoodServings(id);

            if (meal is null)
                return Result
                    .CreateWithError<MealLogResponse>(EvaluationTypes.NotFound, "Meal was not found.");

            var foodIds = meal.FoodServings.Select(x => x.FoodId);
            var foods = (await _foodCatalogService.GetFoodsAsync(foodIds)).Data;

            if(foods is null)
                return Result
                    .CreateWithError<MealLogResponse>(EvaluationTypes.NotFound, "Not all foods were found found.");

            return Result.Create(GetMealLogResponse(meal.FoodServings, foods, meal.Order, meal.EatenOn));
        }

        public async Task<Result<MealLogResponse>> UpdateMealLogAsync(Int32 id, UpdateMealLogRequest request)
        {
            var meal = await _mealRepository.GetMealByIdWithFoodServings(id);

            if (meal is null)
                return Result
                    .CreateWithError<MealLogResponse>(EvaluationTypes.NotFound, "Meal was not found.");

            var foodIds = request.FoodServings.Select(x => x.FoodId);
            var foods = (await _foodCatalogService.GetFoodsAsync(foodIds)).Data;

            if (foods is null)
                return Result
                    .CreateWithError<MealLogResponse>(EvaluationTypes.NotFound, "Not all foods were found found.");

            meal.FoodServings.Clear();

            var foodServings = request.FoodServings
                    .Select(x => new FoodServing
                    {
                        FoodId = x.FoodId,
                        ServingSize = x.Size,
                        ServingUnit = x.Unit,
                        NumberOfServings = x.Number
                    }).ToList();

            foreach (var foodServing in foodServings)
                meal.FoodServings.Add(foodServing);

            await _mealRepository.SaveEntityAsync(meal);

            return Result.Create(GetMealLogResponse(meal.FoodServings, foods, meal.Order, meal.EatenOn));
        }


        public async Task<Result<MealLogResponse>> LogMealAsync(LogMealRequest request)
        {
            var foods = (await _foodCatalogService
                .GetFoodsAsync(request.FoodServings.Select(x => x.FoodId))).Data;

            if (foods is null)
                return Result
                    .CreateWithError<MealLogResponse>(EvaluationTypes.NotFound, "Food with id not found.");

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

            return Result.Create(GetMealLogResponse(foodServings, foods, newMeal.Order, request.Date));
        }

        //foods
        public async Task<Result<FoodLogResponse>> LogFoodAsync(Int32 mealId, LogFoodRequest request)
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

            var meal = await _mealRepository.GetByIdAsync(mealId);

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
        private MealLogResponse GetMealLogResponse(
            IEnumerable<FoodServing> foodServings,
            IReadOnlyCollection<FoodDetails> foods,
            Int32 mealNumber,
            DateTime date)
        {
            return new MealLogResponse
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
                    Nutrition = new LoggedNutrition
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
