using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.DietPlanning.Requests;
using DietAssistant.Business.Contracts.Models.DietPlanning.Responses;
using DietAssistant.Business.Contracts.Models.DietPlanning.Responses.Macros;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
using DietAssistant.Business.Helpers;
using DietAssistant.Business.Mappers;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain.DietPlanning;

namespace DietAssistant.Business
{
    public class DietPlanningService : IDietPlanningService
    {
        private readonly IUserResolverService _userResolverService;
        private readonly IDietPlanRepository _dietPlanRepository;
        private readonly IFoodCatalogService _foodCatalogService;

        public DietPlanningService(
            IUserResolverService userResolverService,
            IDietPlanRepository dietPlanRepository,
            IFoodCatalogService foodCatalogService)
        {
            _userResolverService = userResolverService;
            _dietPlanRepository = dietPlanRepository;
            _foodCatalogService = foodCatalogService;
        }

        public async Task<Result<DietPlanMacrosBreakdownResponse>> GetDietPlanMacrosAsync(Int32 dietPlanId)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<DietPlanMacrosBreakdownResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var dietPlan = await _dietPlanRepository.GetDietPlanAsync(dietPlanId, currentUserId.Value);

            if (dietPlan is null)
                return Result
                    .CreateWithError<DietPlanMacrosBreakdownResponse>(EvaluationTypes.NotFound, "Diet plan was not found.");

            var dayMealPlans = dietPlan.MealPlans
                .GroupBy(mealPlan => 
                    mealPlan.DayOfWeek,
                    (day, mealPlans) => new 
                    {
                        Day = day,
                        MealPlans = mealPlans
                    });

            return await GetResponse(dietPlan);
        }

        //Diet plan
        public async Task<Result<DietPlanResponse>> GetDietPlanAsync(Int32 dietPlanId)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<DietPlanResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var dietPlan = await _dietPlanRepository.GetDietPlanAsync(dietPlanId, currentUserId.Value);

            if (dietPlan is null)
                return Result
                    .CreateWithError<DietPlanResponse>(EvaluationTypes.NotFound, "Diet plan was not found.");

            var req = dietPlan.MealPlans
                .SelectMany(x => x.FoodPlans)
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
                    .CreateWithErrors<DietPlanResponse>(foodsResponse.EvaluationResult, foodsResponse.Errors);

            var foods = foodsResponse.Data;

            var response = dietPlan.ToResponse(foods);

            return Result.Create(response);
        }

        public async Task<Result<Int32>> CreateDietPlanAsync(CreateDietPlanRequest request)
        {
            if (String.IsNullOrEmpty(request.PlanName))
                return Result.CreateWithError<Int32>(EvaluationTypes.InvalidParameters, "The name of the diet plan is required.");

            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<Int32>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var newDietPlan = new DietPlan
            {
                PlanName = request.PlanName,
                UserId = currentUserId.Value
            };

            await _dietPlanRepository.SaveEntityAsync(newDietPlan);

            return newDietPlan.DietPlanId == 0
                ? Result.CreateWithError<Int32>(EvaluationTypes.Failed, "Could not create diet plan.")
                : Result.Create(newDietPlan.DietPlanId);
        }

        public async Task<Result<Int32>> DeleteDietPlanAsync(Int32 dietPlanId)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<Int32>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var dietPlan = await _dietPlanRepository.GetDietPlanAsync(dietPlanId, currentUserId.Value);

            if (dietPlan is null)
                return Result
                    .CreateWithError<Int32>(EvaluationTypes.NotFound, "Diet plan was not found.");

            var result = await _dietPlanRepository.DeleteDietPlanAsync(dietPlan);

            return result <= 0
                ? Result.CreateWithError<Int32>(EvaluationTypes.Failed, "Could not delete food plan.")
                : Result.Create(dietPlan.DietPlanId);
        }

        //Meals
        public async Task<Result<MealPlanResponse>> AddMealPlanAsync(Int32 dietPlanId, AddUpdateMealRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<MealPlanResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var dietPlan = await _dietPlanRepository.GetDietPlanAsync(dietPlanId, currentUserId.Value);

            if (dietPlan is null)
                return Result
                    .CreateWithError<MealPlanResponse>(EvaluationTypes.NotFound, "Diet plan was not found.");

            var req = request.FoodPlanRequests.Select(x => new ManyFoodDetailsRequest
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
                    .CreateWithErrors<MealPlanResponse>(foodsResponse.EvaluationResult, foodsResponse.Errors);

            var newMealPlan = new MealPlan
            {
                MealPlanName = request.MealName,
                DayOfWeek = request.Day,
                Time = request.Time,
                FoodPlans = request.FoodPlanRequests
                    .Select(x =>
                    new FoodPlan
                    {
                        FoodId = x.FoodId,
                        ServingSize = x.ServingSize,
                        Unit = x.Unit
                    })
                    .ToList(),
            };

            dietPlan.MealPlans.Add(newMealPlan);

            await _dietPlanRepository.SaveEntityAsync(dietPlan);

            var response = dietPlan.ToResponse(newMealPlan, foodsResponse.Data);

            return Result.Create(response);
        }

        public async Task<Result<MealPlanResponse>> UpdateMealPlanAsync(Int32 dietPlanId, Int32 mealPlanId, AddUpdateMealRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<MealPlanResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var dietPlan = await _dietPlanRepository.GetDietPlanAsync(dietPlanId, currentUserId.Value);

            if (dietPlan is null)
                return Result
                    .CreateWithError<MealPlanResponse>(EvaluationTypes.NotFound, "Diet plan was not found.");

            var req = request.FoodPlanRequests.Select(x => new ManyFoodDetailsRequest
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
                    .CreateWithErrors<MealPlanResponse>(foodsResponse.EvaluationResult, foodsResponse.Errors);

            var mealPlan = dietPlan.MealPlans.SingleOrDefault(x => x.MealPlanId == mealPlanId);

            if (mealPlan is null)
                return Result
                    .CreateWithError<MealPlanResponse>(EvaluationTypes.NotFound, "Meal plan was not found.");

            mealPlan.MealPlanName = request.MealName;
            mealPlan.Time = request.Time;
            mealPlan.DayOfWeek = request.Day;
            mealPlan.FoodPlans.Clear();

            foreach (var foodPlan in request.FoodPlanRequests)
            {
                mealPlan.FoodPlans.Add(
                    new FoodPlan
                    {
                        FoodId = foodPlan.FoodId,
                        ServingSize = foodPlan.ServingSize,
                        Unit = foodPlan.Unit
                    });
            }

            await _dietPlanRepository.SaveEntityAsync(dietPlan);

            var response = dietPlan.ToResponse(mealPlan, foodsResponse.Data);

            return Result.Create(response);
        }

        public async Task<Result<Int32>> DeleteMealPlanAsync(Int32 dietPlanId, Int32 mealPlanId)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<Int32>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var dietPlan = await _dietPlanRepository.GetDietPlanAsync(dietPlanId, currentUserId.Value);

            if (dietPlan is null)
                return Result
                    .CreateWithError<Int32>(EvaluationTypes.NotFound, "Diet plan was not found.");

            var mealPlan = dietPlan.MealPlans.SingleOrDefault(x => x.MealPlanId == mealPlanId);

            if (mealPlan is null)
                return Result.CreateWithError<Int32>(EvaluationTypes.NotFound, "Meal plan was not found.");

            var result = await _dietPlanRepository.DeleteMealPlanAsync(dietPlan, mealPlan);

            return result <= 0
                ? Result.CreateWithError<Int32>(EvaluationTypes.Failed, "Could not delete food plan.")
                : Result.Create(mealPlan.MealPlanId);
        }

        //Food plan
        public async Task<Result<MealPlanResponse>> AddFoodPlanAsync(Int32 dietPlanId, Int32 mealPlanId, FoodPlanRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<MealPlanResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var dietPlan = await _dietPlanRepository.GetDietPlanAsync(dietPlanId, currentUserId.Value);

            if (dietPlan is null)
                return Result
                    .CreateWithError<MealPlanResponse>(EvaluationTypes.NotFound, "Diet plan was not found.");

            var mealPlan = dietPlan.MealPlans.SingleOrDefault(x => x.MealPlanId == mealPlanId);

            if (mealPlan is null)
                return Result.CreateWithError<MealPlanResponse>(EvaluationTypes.NotFound, "Meal plan was not found.");

            var foodResponse = await _foodCatalogService.GetFoodByIdAsync(request.FoodId,
                new ServingRequest
                {
                    Amount = request.ServingSize,
                    Unit = request.Unit
                });

            if (foodResponse.IsFailure())
                return Result
                    .CreateWithErrors<MealPlanResponse>(foodResponse.EvaluationResult, foodResponse.Errors);

            mealPlan.FoodPlans.Add(new FoodPlan { FoodId = request.FoodId, ServingSize = request.ServingSize, Unit = request.Unit });

            await _dietPlanRepository.SaveEntityAsync(dietPlan);

            var req = mealPlan.FoodPlans.Select(x =>
                new ManyFoodDetailsRequest
                {
                    FoodId = x.FoodId
                });

            var foodsResponse = await _foodCatalogService.GetFoodsAsync(req);

            if (foodsResponse.IsFailure())
                return Result
                    .CreateWithErrors<MealPlanResponse>(foodsResponse.EvaluationResult, foodsResponse.Errors);

            var response = dietPlan.ToResponse(mealPlan, foodsResponse.Data);

            return Result.Create(response);
        }

        public async Task<Result<MealPlanResponse>> UpdateFoodPlanAsync(
            Int32 dietPlanId,
            Int32 mealPlanId,
            Int32 foodPlanId,
            FoodPlanRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<MealPlanResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var dietPlan = await _dietPlanRepository.GetDietPlanAsync(dietPlanId, currentUserId.Value);

            if (dietPlan is null)
                return Result
                    .CreateWithError<MealPlanResponse>(EvaluationTypes.NotFound, "Diet plan was not found.");

            var mealPlan = dietPlan.MealPlans.SingleOrDefault(x => x.MealPlanId == mealPlanId);

            if (mealPlan is null)
                return Result.CreateWithError<MealPlanResponse>(EvaluationTypes.NotFound, "Meal plan was not found.");

            var foodResponse = await _foodCatalogService.GetFoodByIdAsync(request.FoodId, null);

            if (foodResponse.IsFailure())
                return Result
                    .CreateWithErrors<MealPlanResponse>(foodResponse.EvaluationResult, foodResponse.Errors);

            var foodPlan = mealPlan.FoodPlans.SingleOrDefault(x => x.FoodPlanId == foodPlanId);

            if (foodPlan is null)
                return Result.CreateWithError<MealPlanResponse>(EvaluationTypes.NotFound, "Food plan was not found");

            foodPlan.FoodId = request.FoodId;
            foodPlan.ServingSize = request.ServingSize;
            foodPlan.Unit = request.Unit;

            await _dietPlanRepository.SaveEntityAsync(dietPlan);

            var req = mealPlan.FoodPlans.Select(x =>
                new ManyFoodDetailsRequest
                {
                    FoodId = x.FoodId
                });

            var foodsResponse = await _foodCatalogService.GetFoodsAsync(req);

            if (foodsResponse.IsFailure())
                return Result
                    .CreateWithErrors<MealPlanResponse>(foodsResponse.EvaluationResult, foodsResponse.Errors);

            var response = dietPlan.ToResponse(mealPlan, foodsResponse.Data);

            return Result.Create(response);
        }

        public async Task<Result<Int32>> DeleteFoodPlanAsync(Int32 dietPlanId, Int32 mealPlanId, Int32 foodPlanId)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<Int32>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var dietPlan = await _dietPlanRepository.GetDietPlanAsync(dietPlanId, currentUserId.Value);

            if (dietPlan is null)
                return Result
                    .CreateWithError<Int32>(EvaluationTypes.NotFound, "Diet plan was not found.");

            var mealPlan = dietPlan.MealPlans.SingleOrDefault(x => x.MealPlanId == mealPlanId);

            if (mealPlan is null)
                return Result.CreateWithError<Int32>(EvaluationTypes.NotFound, "Meal plan was not found.");


            var foodPlan = mealPlan.FoodPlans.SingleOrDefault(x => x.FoodPlanId == foodPlanId);

            if (foodPlan is null)
                return Result.CreateWithError<Int32>(EvaluationTypes.NotFound, "Food plan was not found");

            mealPlan.FoodPlans.Remove(foodPlan);

            var result = await _dietPlanRepository.DeleteFoodPlanAsync(dietPlan, mealPlan, foodPlan);

            return result <= 0
                ? Result.CreateWithError<Int32>(EvaluationTypes.Failed, "Could not delete food plan.")
                : Result.Create(foodPlan.FoodPlanId);
        }

        private async Task<Result<DietPlanMacrosBreakdownResponse>> GetResponse(DietPlan dietPlan)
        {
            var macrosPerDayResult = await GetTotalMacrosPerDay(dietPlan);

            if (macrosPerDayResult.IsFailure())
                return Result.CreateWithErrors<DietPlanMacrosBreakdownResponse>(macrosPerDayResult.EvaluationResult, macrosPerDayResult.Errors);

            var macrosPerDay = macrosPerDayResult.Data;

            var response = dietPlan.ToResponse(macrosPerDay);

            return Result.Create(response);
        }

        private async Task<Result<List<DayTotalMacros>>> GetTotalMacrosPerDay(DietPlan dietPlan)
        {
            var dayMealPlans = dietPlan.MealPlans
                .GroupBy(mealPlan => mealPlan.DayOfWeek,
                    (day, mealPlans) => new { Day = day, MealPlans = mealPlans });

            var result = new List<DayTotalMacros>();

            foreach (var dayMealPlan in dayMealPlans)
            {
                var dayTotalMacros = new DayTotalMacros
                {
                    Day = dayMealPlan.Day,
                    MealPlansMacros = new List<MealPlanTotalMacrosDto>()
                };

                foreach (var mealPlan in dayMealPlan.MealPlans)
                {
                    var dto = new MealPlanTotalMacrosDto()
                    {
                        MealPlanId = mealPlan.MealPlanId,
                        MealPlanName = mealPlan.MealPlanName
                    };

                    foreach (var foodPlan in mealPlan.FoodPlans)
                    {
                        var req = new ServingRequest
                        {
                            Unit = foodPlan.Unit,
                            Amount = foodPlan.ServingSize
                        };

                        var foodResponse = await _foodCatalogService.GetFoodByIdAsync(foodPlan.FoodId, req);

                        if (foodResponse.IsFailure())
                            return Result
                                .CreateWithErrors<List<DayTotalMacros>>(foodResponse.EvaluationResult, foodResponse.Errors);

                        var food = foodResponse.Data;

                        var foodCalories = food.Nutrition.GetNutrientAmount(DietAssistantConstants.Calories);
                        var foodCarbs = food.Nutrition.GetNutrientAmount(DietAssistantConstants.Carbohydrates);
                        var foodProtein = food.Nutrition.GetNutrientAmount(DietAssistantConstants.Protein);
                        var foodFat = food.Nutrition.GetNutrientAmount(DietAssistantConstants.Fat);

                        dto.TotalCalories += foodCalories;
                        dto.TotalCarbs += foodCarbs;
                        dto.TotalProtein += foodProtein;
                        dto.TotalFat += foodFat;
                    }

                    dayTotalMacros.MealPlansMacros.Add(dto);
                }
                result.Add(dayTotalMacros);
            }

            return Result.Create(result);
        }
    }
}
