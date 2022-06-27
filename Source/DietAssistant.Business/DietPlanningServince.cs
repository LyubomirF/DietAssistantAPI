using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.DietPlanning.Requests;
using DietAssistant.Business.Contracts.Models.DietPlanning.Responses;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain.DietPlanning;

namespace DietAssistant.Business
{
    public class DietPlanningServince
    {
        private readonly IUserResolverService _userResolverService;
        private readonly IDietPlanRepository _dietPlanRepository;
        private readonly IFoodCatalogService _foodCatalogService;

        public DietPlanningServince(
            IUserResolverService userResolverService,
            IDietPlanRepository dietPlanRepository,
            IFoodCatalogService foodCatalogService)
        {
            _userResolverService = userResolverService;
            _dietPlanRepository = dietPlanRepository;
            _foodCatalogService = foodCatalogService;
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

            var response = GetResponse(dietPlan, foods);

            return Result.Create(response);
        }

        public async Task<Result<Int32>> CreateDietPlan(String planName)
        {
            if (String.IsNullOrEmpty(planName))
                return Result.CreateWithError<Int32>(EvaluationTypes.InvalidParameters, "The name of the diet plan is required.");

            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<Int32>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var newDietPlan = new DietPlan
            {
                PlanName = planName,
                UserId = currentUserId.Value
            };

            await _dietPlanRepository.SaveEntityAsync(newDietPlan);

            return newDietPlan.DietPlanId == 0
                ? Result.CreateWithError<Int32>(EvaluationTypes.Failed, "Could not create diet plan.")
                : Result.Create(newDietPlan.DietPlanId);
        }

        //Delete diet plan

        //Meals
        public async Task<Result<MealPlanResponse>> AddMealToPlanAsync(Int32 dietPlanId, AddUpdateMealRequest request)
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

            var newMeal = new MealPlan
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

            dietPlan.MealPlans.Add(newMeal);

            await _dietPlanRepository.SaveEntityAsync(dietPlan);

            var response = GetResponse(dietPlan, newMeal, foodsResponse.Data);

            return Result.Create(response);
        }

        public async Task<Result<MealPlanResponse>> UpdateMealAsync(Int32 dietPlanId, Int32 mealId, AddUpdateMealRequest request)
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

            var meal = dietPlan.MealPlans.SingleOrDefault(x => x.MealPlanId == mealId);

            if (meal is null)
                return Result
                    .CreateWithError<MealPlanResponse>(EvaluationTypes.NotFound, "Meal was not found.");

            meal.FoodPlans.Clear();

            foreach (var foodPlan in request.FoodPlanRequests)
            {
                meal.FoodPlans.Add(
                    new FoodPlan
                    {
                        FoodId = foodPlan.FoodId,
                        ServingSize = foodPlan.ServingSize,
                        Unit = foodPlan.Unit
                    });
            }

            await _dietPlanRepository.SaveEntityAsync(dietPlan);

            var response = GetResponse(dietPlan, meal, foodsResponse.Data);

            return Result.Create(response);
        }
        // Delete meal plan from diet plan ...

        //Food plan
        public async Task<Result<MealPlanResponse>> AddFoodPlanToMeal(Int32 dietPlanId, Int32 mealId, FoodPlanRequest request)
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result.CreateWithError<MealPlanResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var dietPlan = await _dietPlanRepository.GetDietPlanAsync(dietPlanId, currentUserId.Value);

            if (dietPlan is null)
                return Result
                    .CreateWithError<MealPlanResponse>(EvaluationTypes.NotFound, "Diet plan was not found.");

            var mealPlan = dietPlan.MealPlans.SingleOrDefault(x => x.MealPlanId == mealId);

            if (mealPlan is null)
                return Result.CreateWithError<MealPlanResponse>(EvaluationTypes.NotFound, "Meal plan was not found.");

            var foodResponse = await _foodCatalogService.GetFoodByIdAsync(request.FoodId, null);

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

            var response = GetResponse(dietPlan, mealPlan, foodsResponse.Data);

            return Result.Create(response);
        }

        public async Task<Result<MealPlanResponse>> UpdateFoodPlan(
            Int32 dietPlanId,
            Int32 mealId,
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

            var mealPlan = dietPlan.MealPlans.SingleOrDefault(x => x.MealPlanId == mealId);

            if (mealPlan is null)
                return Result.CreateWithError<MealPlanResponse>(EvaluationTypes.NotFound, "Meal plan was not found.");

            var foodResponse = await _foodCatalogService.GetFoodByIdAsync(request.FoodId, null);

            if (foodResponse.IsFailure())
                return Result
                    .CreateWithErrors<MealPlanResponse>(foodResponse.EvaluationResult, foodResponse.Errors);

            var foodPlan = mealPlan.FoodPlans.SingleOrDefault(x => x.FoodPlanId == foodPlanId);

            if(foodPlan is null)
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

            var response = GetResponse(dietPlan, mealPlan, foodsResponse.Data);

            return Result.Create(response);
        }

        //Delete food plan

        private MealPlanResponse GetResponse(DietPlan dietPlan, MealPlan meal, IReadOnlyCollection<FoodDetails> foods)
        {
            return new MealPlanResponse
            {
                DietPlanId = dietPlan.DietPlanId,
                MealPlanId = meal.MealPlanId,
                DayOfWeek = meal.DayOfWeek.ToString(),
                Time = meal.Time.ToString(@"hh\:mm"),
                FoodPlan = meal.FoodPlans
                    .Zip(foods, (fp, f) => new { FoodPlan = fp, Food = f })
                    .Select(x =>
                        new FoodPlanResponse
                        {
                            FoodPlanId = x.FoodPlan.FoodPlanId,
                            FoodId = x.Food.FoodId,
                            FoodName = x.Food.FoodName,
                            ServingSize = x.FoodPlan.ServingSize,
                            Unit = x.FoodPlan.Unit
                        })
                    .ToList()
            };
        }

        private DietPlanResponse GetResponse(DietPlan dietPlan, IReadOnlyCollection<FoodDetails> foods)
        {
            return new DietPlanResponse
            {
                DietPlanId = dietPlan.DietPlanId,
                Name = dietPlan.PlanName,
                DayPlans = dietPlan.MealPlans
                    .GroupBy(x => x.DayOfWeek, (day, mealPlans) =>
                        new DayPlanResponse
                        {
                            DayOfWeek = day.ToString(),
                            Meals = mealPlans
                                .Select(mealPlan =>
                                    new SimpleMealPlanResponse
                                    {
                                        MealPlanId = mealPlan.MealPlanId,
                                        MealName = mealPlan.MealPlanName,
                                        Time = mealPlan.Time.ToString(@"hh\:mm"),
                                        FoodPlan = mealPlan.FoodPlans
                                            .Select(fp =>
                                                new FoodPlanResponse
                                                {
                                                    FoodPlanId = fp.FoodPlanId,
                                                    FoodId = fp.FoodId,
                                                    FoodName = foods.SingleOrDefault(x => x.FoodId == fp.FoodId)?.FoodName,
                                                    ServingSize = fp.ServingSize,
                                                    Unit = fp.Unit
                                                }).ToList()
                                    }).ToList()
                        }).ToList()
            };
        }
    }
}
