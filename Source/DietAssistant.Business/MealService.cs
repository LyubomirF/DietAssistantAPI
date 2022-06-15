using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Business.Contracts.Models.FoodServing.Responses;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Responses;
using DietAssistant.Business.Helpers;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;

namespace DietAssistant.Business
{
    public class MealService : IMealService
    {
        private readonly IUserResolverService _userResolverService;
        private readonly IFoodCatalogService _foodCatalogService;
        private readonly IMealRepository _mealRepository;

        public MealService(
            IUserResolverService userResolverService,
            IFoodCatalogService foodCatalogService,
            IMealRepository mealRepository)
        {
            _userResolverService = userResolverService;
            _foodCatalogService = foodCatalogService;
            _mealRepository = mealRepository;
        }

        public async Task<Result<MealLogResponse>> GetMealById(Int32 id)
        {
            int? currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<MealLogResponse>(EvaluationTypes.Unauthorized, "Unauthorized.");

            Meal? meal = await _mealRepository.GetMealByIdWithFoodServings(id, currentUserId.Value);

            if (meal is null)
                return Result
                    .CreateWithError<MealLogResponse>(EvaluationTypes.NotFound, "Meal was not found.");

            IEnumerable<int>? foodIds = meal.FoodServings.Select(x => x.FoodId);
            IReadOnlyCollection<FoodDetails>? foods = (await _foodCatalogService.GetFoodsAsync(foodIds)).Data;

            if (foods is null)
                return Result
                    .CreateWithError<MealLogResponse>(EvaluationTypes.NotFound, "Not all foods were found.");

            return Result.Create(GetMealLogResponse(meal.FoodServings, foods, meal));
        }

        public async Task<Result<MealLogResponse>> LogMealAsync(LogMealRequest request)
        {
            int? currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<MealLogResponse>(EvaluationTypes.Unauthorized, "Unauthorized.");

            IReadOnlyCollection<FoodDetails>? foods = (await _foodCatalogService
                .GetFoodsAsync(request.FoodServings.Select(x => x.FoodId))).Data;

            if (foods is null)
                return Result
                    .CreateWithError<MealLogResponse>(EvaluationTypes.NotFound, "Food with id not found.");

            Meal? lastMeal = await _mealRepository.GetLastMealAsync(request.Date, currentUserId.Value);

            List<FoodServing>? foodServings = request.FoodServings
                    .Select(x => new FoodServing
                    {
                        FoodId = x.FoodId,
                        ServingSize = x.ServingSize,
                        ServingUnit = x.Unit,
                        NumberOfServings = x.NumberOfServings
                    }).ToList();

            Meal? newMeal = new Meal
            {
                UserId = currentUserId.Value
            };

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

            return Result.Create(GetMealLogResponse(foodServings, foods, newMeal));
        }

        public async Task<Result<MealLogResponse>> UpdateMealLogAsync(Int32 id, UpdateMealLogRequest request)
        {
            int? currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<MealLogResponse>(EvaluationTypes.Unauthorized, "Unauthorized.");

            Meal? meal = await _mealRepository.GetMealByIdWithFoodServings(id, currentUserId.Value);

            if (meal is null)
                return Result
                    .CreateWithError<MealLogResponse>(EvaluationTypes.NotFound, "Meal was not found.");

            IEnumerable<int>? foodIds = request.FoodServings.Select(x => x.FoodId);
            IReadOnlyCollection<FoodDetails>? foods = (await _foodCatalogService.GetFoodsAsync(foodIds)).Data;

            if (foods is null)
                return Result
                    .CreateWithError<MealLogResponse>(EvaluationTypes.NotFound, "Not all foods were found found.");

            meal.FoodServings.Clear();

            List<FoodServing>? foodServings = request.FoodServings
                    .Select(x => new FoodServing
                    {
                        FoodId = x.FoodId,
                        ServingSize = x.ServingSize,
                        ServingUnit = x.Unit,
                        NumberOfServings = x.NumberOfServings
                    }).ToList();

            foreach (FoodServing? foodServing in foodServings)
                meal.FoodServings.Add(foodServing);

            await _mealRepository.SaveEntityAsync(meal);

            return Result.Create(GetMealLogResponse(meal.FoodServings, foods, meal));
        }

        public async Task<Result<Int32>> DeleteMealAsync(Int32 id)
        {
            int? currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<Int32>(EvaluationTypes.Unauthorized, "Unauthorized.");

            Meal? meal = await _mealRepository.GetMealByIdWithFoodServings(id, currentUserId.Value);

            if (meal is null)
                return Result.CreateWithError<Int32>(EvaluationTypes.NotFound, "Meal was not found.");

            int result = await _mealRepository.DeleteMealAsync(meal);

            return result <= 0
                ? Result.CreateWithError<Int32>(EvaluationTypes.Failed, "Couldn't delete meal.")
                : Result.Create(id);
        }

        private MealLogResponse GetMealLogResponse(
            IEnumerable<FoodServing> foodServings,
            IReadOnlyCollection<FoodDetails> foods,
            Meal meal)
        {
            List<LoggedFoodServing>? loggedFood = foodServings
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
                        Carbs = x.Food.CalculateNutrientTotal(x.FoodServing, DietAssistantConstants.Carbohydrates),
                        Fat = x.Food.CalculateNutrientTotal(x.FoodServing, DietAssistantConstants.Fat),
                        Protein = x.Food.CalculateNutrientTotal(x.FoodServing, DietAssistantConstants.Protein),
                        Calories = x.Food.CalculateNutrientTotal(x.FoodServing, DietAssistantConstants.Calories),
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
