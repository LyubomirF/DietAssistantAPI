using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Business.Contracts.Models.FoodServing.Requests;
using DietAssistant.Business.Contracts.Models.FoodServing.Responses;
using DietAssistant.Business.Helpers;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;

namespace DietAssistant.Business
{
    public class FoodServingService : IFoodServingService
    {
        private readonly IUserResolverService _userResolverService;
        private readonly IFoodCatalogService _foodCatalogService;
        private readonly IMealRepository _mealRepository;

        public FoodServingService(
            IUserResolverService userResolverService,
            IFoodCatalogService foodCatalogService,
            IMealRepository mealRepository)
        {
            _userResolverService = userResolverService;
            _foodCatalogService = foodCatalogService;
            _mealRepository = mealRepository;
        }

        public async Task<Result<FoodServingResponse>> LogFoodServingAsync(Int32 mealId, LogUpdateFoodServingRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<FoodServingResponse>(EvaluationTypes.Unauthorized, "Unauthorized.");

            var food = (await _foodCatalogService.GetFoodByIdAsync(request.FoodId)).Data;

            if (food is null)
                return Result
                    .CreateWithError<FoodServingResponse>(EvaluationTypes.NotFound, "Food with id not found.");

            var foodServing = new FoodServing
            {
                FoodId = request.FoodId,
                NumberOfServings = request.NumberOfServings,
                ServingSize = request.ServingSize,
                ServingUnit = request.Unit
            };

            var meal = await _mealRepository.GetMealByIdWithFoodServings(mealId, currentUserId.Value);

            if (meal is null)
                return Result
                    .CreateWithError<FoodServingResponse>(EvaluationTypes.NotFound, "Meal not found.");

            meal.FoodServings.Add(foodServing);

            await _mealRepository.SaveEntityAsync(meal);

            return Result.Create(GetFoodLogResponse(meal, food, foodServing));
        }

        public async Task<Result<FoodServingResponse>> UpdateFoodServingLogAsync(
            Int32 mealId,
            Int32 foodServingId,
            LogUpdateFoodServingRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<FoodServingResponse>(EvaluationTypes.Unauthorized, "Unauthorized.");

            var meal = await _mealRepository.GetMealByIdWithFoodServings(mealId, currentUserId.Value);

            if (meal is null)
                return Result
                    .CreateWithError<FoodServingResponse>(EvaluationTypes.NotFound, "Meal was not found.");

            var foodServing = meal.FoodServings.SingleOrDefault(x => x.FoodServingId == foodServingId);

            if (foodServing is null)
                return Result
                    .CreateWithError<FoodServingResponse>(EvaluationTypes.NotFound, "Food serving was not found.");

            foodServing.ServingUnit = request.Unit;
            foodServing.ServingSize = request.ServingSize;
            foodServing.NumberOfServings = request.NumberOfServings;

            await _mealRepository.SaveEntityAsync(meal);

            var food = (await _foodCatalogService.GetFoodByIdAsync(foodServing.FoodId)).Data;

            return Result.Create(GetFoodLogResponse(meal, food, foodServing));
        }

        public async Task<Result<Int32>> DeleteFoodServingLogAsync(Int32 mealId, Int32 foodServingId)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<Int32>(EvaluationTypes.Unauthorized, "Unauthorized.");

            var meal = await _mealRepository.GetMealByIdWithFoodServings(mealId, currentUserId.Value);

            if (meal is null)
                return Result
                    .CreateWithError<Int32>(EvaluationTypes.NotFound, "Meal was not found.");

            var foodServing = meal.FoodServings.SingleOrDefault(x => x.FoodServingId == foodServingId);

            if (foodServing is null)
                return Result
                    .CreateWithError<Int32>(EvaluationTypes.NotFound, "Food serving was not found.");

            var result = await _mealRepository.DeleteFoodServingAsync(meal, foodServing);

            return result <= 0
                ? Result.CreateWithError<Int32>(EvaluationTypes.Failed, "Couldn't delete food serving.")
                : Result.Create(foodServingId);
        }

        private FoodServingResponse GetFoodLogResponse(Meal meal, FoodDetails food, FoodServing foodServing)
        {
            return new FoodServingResponse
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
                        Calories = food.CalculateNutrientTotal(foodServing, DietAssistantConstants.Calories),
                        Carbs = food.CalculateNutrientTotal(foodServing, DietAssistantConstants.Carbohydrates),
                        Fat = food.CalculateNutrientTotal(foodServing, DietAssistantConstants.Fat),
                        Protein = food.CalculateNutrientTotal(foodServing, DietAssistantConstants.Protein),
                    }
                }
            };
        }
    }
}
