using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
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
                return Result.CreateWithError<FoodServingResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var foodResponse = await _foodCatalogService.GetFoodByIdAsync(
                request.FoodId,
                new ServingRequest
                {
                    Amount = request.ServingSize,
                    Unit = request.Unit
                });

            if (foodResponse.IsFailure())
                return Result
                    .CreateWithErrors<FoodServingResponse>(foodResponse.EvaluationResult, foodResponse.Errors);

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
                    .CreateWithError<FoodServingResponse>(EvaluationTypes.NotFound, ResponseMessages.MealNotFound(mealId));

            meal.FoodServings.Add(foodServing);

            await _mealRepository.SaveEntityAsync(meal);

            return Result.Create(GetFoodLogResponse(meal, foodResponse.Data, foodServing));
        }

        public async Task<Result<FoodServingResponse>> UpdateFoodServingLogAsync(
            Int32 mealId,
            Int32 foodServingId,
            LogUpdateFoodServingRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<FoodServingResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var meal = await _mealRepository.GetMealByIdWithFoodServings(mealId, currentUserId.Value);

            if (meal is null)
                return Result
                    .CreateWithError<FoodServingResponse>(EvaluationTypes.NotFound, ResponseMessages.MealNotFound(mealId));

            var foodServing = meal.FoodServings.SingleOrDefault(x => x.FoodServingId == foodServingId);

            if (foodServing is null)
                return Result
                    .CreateWithError<FoodServingResponse>(EvaluationTypes.NotFound, ResponseMessages.FoodServingNotFound(foodServingId));

            foodServing.ServingUnit = request.Unit;
            foodServing.ServingSize = request.ServingSize;
            foodServing.NumberOfServings = request.NumberOfServings;

            await _mealRepository.SaveEntityAsync(meal);

            var foodResponse = await _foodCatalogService.GetFoodByIdAsync(foodServing.FoodId, new ServingRequest
            {
                Amount = request.ServingSize,
                Unit = request.Unit
            });

            if(foodResponse.IsFailure())
               return Result
                    .CreateWithErrors<FoodServingResponse>(foodResponse.EvaluationResult, foodResponse.Errors);

            return Result.Create(GetFoodLogResponse(meal, foodResponse.Data, foodServing));
        }

        public async Task<Result<Int32>> DeleteFoodServingLogAsync(Int32 mealId, Int32 foodServingId)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<Int32>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var meal = await _mealRepository.GetMealByIdWithFoodServings(mealId, currentUserId.Value);

            if (meal is null)
                return Result
                    .CreateWithError<Int32>(EvaluationTypes.NotFound, ResponseMessages.FoodServingNotFound(mealId));

            var foodServing = meal.FoodServings.SingleOrDefault(x => x.FoodServingId == foodServingId);

            if (foodServing is null)
                return Result
                    .CreateWithError<Int32>(EvaluationTypes.NotFound, ResponseMessages.FoodServingNotFound(foodServingId));

            var result = await _mealRepository.DeleteFoodServingAsync(meal, foodServing);

            return result <= 0
                ? Result.CreateWithError<Int32>(EvaluationTypes.Failed, ResponseMessages.CannotDeleteFoodServing)
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
                        Calories = NutritionHelper.CalculateNutrientAmount(
                            food.Nutrition,
                            DietAssistantConstants.Calories,
                            foodServing.NumberOfServings),
                        Carbs = NutritionHelper.CalculateNutrientAmount(
                            food.Nutrition,
                            DietAssistantConstants.Carbohydrates,
                            foodServing.NumberOfServings),
                        Fat = NutritionHelper.CalculateNutrientAmount(
                            food.Nutrition,
                            DietAssistantConstants.Fat,
                            foodServing.NumberOfServings),
                        Protein = NutritionHelper.CalculateNutrientAmount(
                            food.Nutrition,
                            DietAssistantConstants.Protein,
                            foodServing.NumberOfServings)
                    }
                }
            };
        }
    }
}
