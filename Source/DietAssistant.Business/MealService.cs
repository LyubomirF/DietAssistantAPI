using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Business.Contracts.Models.FoodServing.Responses;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Responses;
using DietAssistant.Business.Helpers;
using DietAssistant.Business.Mappers;
using DietAssistant.Business.Validation;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;

namespace DietAssistant.Business
{
    using static Validator;

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

        public async Task<Result<IEnumerable<MealLogResponse>>> GetMealsOnDateAsync(DateTime? dateRequest)
        {
            if (!dateRequest.HasValue)
            {
                dateRequest = DateTime.UtcNow;
            }

            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<IEnumerable<MealLogResponse>>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var meals = await _mealRepository.GetMealsForDayAsync(dateRequest.Value, currentUserId.Value);

            return await GetMealLogResponses(meals);
        }

        public async Task<Result<MealLogResponse>> GetMealById(Int32 id)
        {
            if (!Validate(id, out string error))
                return Result.CreateWithError<MealLogResponse>(EvaluationTypes.InvalidParameters, error);

            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<MealLogResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var meal = await _mealRepository.GetMealByIdWithFoodServings(id, currentUserId.Value);

            if (meal is null)
                return Result
                    .CreateWithError<MealLogResponse>(EvaluationTypes.NotFound, ResponseMessages.MealNotFound(id));

            var req = meal.FoodServings.Select(x => new ManyFoodDetailsRequest
            {
                FoodId = x.FoodId,
                Serving = new ServingRequest
                {
                    Unit = x.ServingUnit,
                    Amount = x.ServingSize
                }
            });

            var foodsResponse = await _foodCatalogService.GetFoodsAsync(req);

            if (foodsResponse.IsFailure())
                return Result
                    .CreateWithErrors<MealLogResponse>(foodsResponse.EvaluationResult, foodsResponse.Errors);

            return Result.Create(meal.ToResponse(meal.FoodServings, foodsResponse.Data));
        }

        public async Task<Result<MealLogResponse>> LogMealAsync(LogMealRequest request)
        {
            if (!Validate(request, out List<String> errors))
                return Result.CreateWithErrors<MealLogResponse>(EvaluationTypes.InvalidParameters, errors);

            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<MealLogResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var req = request.FoodServings
                .Select(x => new ManyFoodDetailsRequest
                {
                    FoodId = x.FoodId,
                    Serving = new ServingRequest
                    {
                        Unit = x.Unit,
                        Amount = x.ServingSize
                    }
                });

            var foodsResponse = await _foodCatalogService.GetFoodsAsync(req);

            if (foodsResponse.IsFailure())
                return Result
                    .CreateWithErrors<MealLogResponse>(foodsResponse.EvaluationResult, foodsResponse.Errors);

            var lastMeal = await _mealRepository.GetLastMealAsync(request.Date, currentUserId.Value);

            var foodServings = request.FoodServings
                    .Select(x => new FoodServing
                    {
                        FoodId = x.FoodId,
                        ServingSize = x.ServingSize,
                        ServingUnit = x.Unit,
                        NumberOfServings = x.NumberOfServings
                    }).ToList();

            var newMeal = new Meal
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

            return Result.Create(newMeal.ToResponse(foodServings, foodsResponse.Data));
        }

        public async Task<Result<MealLogResponse>> UpdateMealLogAsync(Int32 id, UpdateMealLogRequest request)
        {
            if (!Validate(id, request, out List<String> errors))
                return Result.CreateWithErrors<MealLogResponse>(EvaluationTypes.InvalidParameters, errors);

            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<MealLogResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var meal = await _mealRepository.GetMealByIdWithFoodServings(id, currentUserId.Value);

            if (meal is null)
                return Result
                    .CreateWithError<MealLogResponse>(EvaluationTypes.NotFound, "Meal was not found.");

            var req = request.FoodServings.Select(x => new ManyFoodDetailsRequest
            {
                FoodId = x.FoodId,
                Serving = new ServingRequest
                {
                    Unit = x.Unit,
                    Amount = x.ServingSize
                }
            });
            var foodsResponse = await _foodCatalogService.GetFoodsAsync(req);

            if (foodsResponse.IsFailure())
                return Result
                    .CreateWithErrors<MealLogResponse>(foodsResponse.EvaluationResult, foodsResponse.Errors);

            meal.FoodServings.Clear();

            var foodServings = request.FoodServings
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

            return Result.Create(meal.ToResponse(meal.FoodServings, foodsResponse.Data));
        }

        public async Task<Result<Int32>> DeleteMealAsync(Int32 id)
        {
            if (!Validate(id, out String error))
                return Result.CreateWithError<Int32>(EvaluationTypes.InvalidParameters, error);

            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<Int32>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var meal = await _mealRepository.GetMealByIdWithFoodServings(id, currentUserId.Value);

            if (meal is null)
                return Result.CreateWithError<Int32>(EvaluationTypes.NotFound, ResponseMessages.MealNotFound(id));

            var result = await _mealRepository.DeleteMealAsync(meal);

            return result <= 0
                ? Result.CreateWithError<Int32>(EvaluationTypes.Failed, ResponseMessages.CannotDeleteMeal)
                : Result.Create(id);
        }

        private async Task<Result<IEnumerable<MealLogResponse>>> GetMealLogResponses(IEnumerable<Meal> meals)
        {
            var result = new List<MealLogResponse>();

            foreach (var meal in meals)
            {
                var req = meal.FoodServings.Select(x => new ManyFoodDetailsRequest
                {
                    FoodId = x.FoodId,
                    Serving = new ServingRequest
                    {
                        Unit = x.ServingUnit,
                        Amount = x.ServingSize
                    }
                });

                var foodsResponse = await _foodCatalogService.GetFoodsAsync(req);

                if (foodsResponse.IsFailure())
                    return Result
                        .CreateWithErrors<IEnumerable<MealLogResponse>>(foodsResponse.EvaluationResult, foodsResponse.Errors);

                result.Add(meal.ToResponse(meal.FoodServings, foodsResponse.Data));
            }

            return Result.Create(result.AsEnumerable());
        }
    }
}
