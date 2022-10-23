using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Responses;
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
        private readonly IUserRepository _userRepository;
        private readonly IFoodCatalogService _foodCatalogService;
        private readonly IMealRepository _mealRepository;

        public MealService(
            IUserResolverService userResolverService,
            IUserRepository userRepository,
            IFoodCatalogService foodCatalogService,
            IMealRepository mealRepository)
        {
            _userResolverService = userResolverService;
            _foodCatalogService = foodCatalogService;
            _mealRepository = mealRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<IEnumerable<MealLogResponse>>> GetMealsOnDateAsync(DateTime? dateRequest)
        {
            if (!dateRequest.HasValue)
            {
                dateRequest = DateTime.Today;
            }

            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<IEnumerable<MealLogResponse>>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var meals = await _mealRepository.GetMealsForDayAsync(dateRequest.Value, currentUserId.Value);

            return Result.Create(await GetMealLogResponses(meals));
        }

        public async Task<Result<DayCaloriesProgress>> GetCaloriesBreakdownAsync(DateTime? date) 
        {
            if (!date.HasValue)
            {
                date = DateTime.Today;
            }

            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<DayCaloriesProgress>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);

            var meals = await _mealRepository.GetMealsForDayAsync(date.Value, currentUserId.Value);

            var mealsBreakdown = await GetMealLogResponses(meals);

            return Result.Create(GetCaloriesBreakdown(mealsBreakdown, user.Goal));
        }

        public async Task<Result<DayMacrosProgress>> GetMacrosBreakdownAsync(DateTime? date)
        {
            if (!date.HasValue)
            {
                date = DateTime.Today;
            }

            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<DayMacrosProgress>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(currentUserId.Value);

            var meals = await _mealRepository.GetMealsForDayAsync(date.Value, currentUserId.Value);
            var mealsBreakdown = await GetMealLogResponses(meals);

            return Result.Create(GetMacrosBreakdown(mealsBreakdown, user.Goal));
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

        private async Task<IEnumerable<MealLogResponse>> GetMealLogResponses(IEnumerable<Meal> meals)
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
                    continue;

                result.Add(meal.ToResponse(meal.FoodServings, foodsResponse.Data));
            }

            return result.AsEnumerable();
        }

        private DayCaloriesProgress GetCaloriesBreakdown(IEnumerable<MealLogResponse> mealLogs, Goal goal)
        {
            var dayLoggedCalories = mealLogs
                .Select(x => x.TotalCalories)
                .Aggregate((x, y) => x + y);

            var mealCalories = mealLogs
                .Select(x => new MealCalories
                {
                    MealId = x.MealId,
                    MealNumber = x.MealNumber,
                    Calories = x.TotalCalories,
                    PercentOfTotal = Math.Round(x.TotalCalories / dayLoggedCalories * 100, 2)
                });

            var dayCaloriesProgress = new DayCaloriesProgress
            {
                MealCalories = mealCalories,
                LoggedCalories = dayLoggedCalories,
                GoalCalories = goal.NutritionGoal.Calories,
            };

            return dayCaloriesProgress;
        }

        private DayMacrosProgress GetMacrosBreakdown(IEnumerable<MealLogResponse> mealLogs, Goal goal)
        {
            var dayLoggedCalories = mealLogs
                .Select(x => x.TotalCalories)
                .Aggregate((x, y) => x + y);

            var carbsLogged = mealLogs
                .Select(x => x.TotalCarbs)
                .Aggregate((x, y) => x + y);

            var proteinLogged = mealLogs
                .Select(x => x.TotalProtein)
                .Aggregate((x, y) => x + y);

            var fatsLogged = mealLogs
                .Select(x => x.TotalFat)
                .Aggregate((x, y) => x + y);

            var loggedCarbsPercentage = Math.Round(carbsLogged * 4 / dayLoggedCalories * 100, 2);
            var loggedProteinPercentage = Math.Round(proteinLogged * 4 / dayLoggedCalories * 100, 2);
            var loggedFatsPercentage = Math.Round(fatsLogged * 9 / dayLoggedCalories * 100, 2);

            var goalCalories = goal.NutritionGoal.Calories;

            var carbsGoalPercentage = goal.NutritionGoal.PercentCarbs;
            var proteinGoalPercentage = goal.NutritionGoal.PercentProtein;
            var fatsGoalPercentage = goal.NutritionGoal.PercentFat;

            var carbsGoal = Math.Round(goalCalories * carbsGoalPercentage / 100 / 4, 2);
            var proteinGoal = Math.Round(goalCalories * proteinGoalPercentage / 100 / 4, 2);
            var fatsGoal = Math.Round(goalCalories * fatsGoalPercentage / 100 / 9, 2);

            return new DayMacrosProgress
            {
                CarbsLogged = carbsLogged,
                ProteinLogged = proteinLogged,
                FatsLogged = fatsLogged,
                CarbsPercentage = loggedCarbsPercentage,
                ProteinPercentage = loggedProteinPercentage,
                FatsPercentage = loggedFatsPercentage,
                CarbsGoal = carbsGoal,
                ProteinGoal = proteinGoal,
                FatsGoal = fatsGoal,
                CarbsGoalPercentage = carbsGoalPercentage,
                ProteinGoalPercentage = proteinGoalPercentage,
                FatsGoalPercentage = fatsGoalPercentage
            };
        }
    }
}
