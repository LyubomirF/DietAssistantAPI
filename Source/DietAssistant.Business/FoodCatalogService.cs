using DietAssistant.Business.Configuration;
using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace DietAssistant.Business
{
    public class FoodCatalogService : IFoodCatalogService
    {
        private readonly RestClient _restClient;

        public FoodCatalogService(IOptions<NutritionApiConfiguration> options)
        {
            var config = options.Value;
            _restClient = new RestClient(NutritionApiRoutes.BaseUrl)
                .AddDefaultHeader("X-RapidAPI-Host", config.Host)
                .AddDefaultHeader("X-RapidAPI-Key", config.Key);
        }

        public async Task<Result<FoodSearch>> SearchFoodsAsync(SearchFoodRequest requestModel)
        {
            var request = GetSearchFoodsRestRequest(requestModel);

            var response = await _restClient.GetAsync(request);

            if (response is null)
                return Result
                    .CreateWithError<FoodSearch>(EvaluationTypes.Failed, "Unable to fetch results.");

            var data = JsonToFoodSearch(response.Content);

            if (data is null)
                return Result
                    .CreateWithError<FoodSearch>(EvaluationTypes.Failed, "Unable to fetch results.");

            return Result.Create(data);
        }

        public async Task<Result<FoodDetails>> GetFoodByIdAsync(Int32 id)
        {
            var request = new RestRequest(NutritionApiRoutes.GetFood(id));

            var response = await _restClient.GetAsync(request);

            if (response is null)
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.Failed, "Unable to fetch results.");

            var data = JsonToFoodDetails(response.Content);

            if (data is null)
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.Failed, "Unable to fetch results.");

            return Result.Create(data);
        }

        public async Task<Result<IReadOnlyCollection<FoodDetails>>> GetFoodsAsync(IEnumerable<Int32> foodIds)
        {
            var result = new List<FoodDetails>();

            foreach (var foodId in foodIds)
            {
                var foodResponse = await GetFoodByIdAsync(foodId);

                if (foodResponse.IsFailure())
                    return Result
                        .CreateWithError<IReadOnlyCollection<FoodDetails>>(EvaluationTypes.NotFound, "Food with id was not found");

                result.Add(foodResponse.Data);
            }

            return Result.Create<IReadOnlyCollection<FoodDetails>>(result);
        }

        private RestRequest GetSearchFoodsRestRequest(SearchFoodRequest request)
        {
            var restRequest = new RestRequest(NutritionApiRoutes.SearchFoods);

            restRequest
                .AddQueryParameter("offset", request.Page)
                .AddQueryParameter("number", request.PageSize)
                .AddQueryParameter("query", request.SearchQuery);

            if (request.MinCalories.HasValue)
                restRequest.AddQueryParameter("minCalories", request.MinCalories.Value);

            if (request.MaxCalories.HasValue)
                restRequest.AddQueryParameter("maxCalories", request.MaxCalories.Value);

            if (request.MinCarbs.HasValue)
                restRequest.AddQueryParameter("minCarbs", request.MinCarbs.Value);

            if (request.MaxCarbs.HasValue)
                restRequest.AddQueryParameter("maxCarbs", request.MaxCarbs.Value);

            if (request.MinFat.HasValue)
                restRequest.AddQueryParameter("minFat", request.MinFat.Value);

            if (request.MaxFat.HasValue)
                restRequest.AddQueryParameter("maxFat", request.MaxFat.Value);

            if (request.MinProtein.HasValue)
                restRequest.AddQueryParameter("minProtein", request.MinProtein.Value);

            if (request.MaxProtein.HasValue)
                restRequest.AddQueryParameter("maxProtein", request.MaxProtein.Value);

            return restRequest;
        }

        private FoodDetails JsonToFoodDetails(string json)
        {
            var definition = new
            {
                Id = 1,
                Title = "",
                Description = "",
                Image = "",
                Servings = new
                {
                    Number = 1.0,
                    Size = 1.0,
                    Unit = ""
                },
                Nutrition = new
                {
                    Nutrients = new[]
                    {
                        new {
                            Name = "",
                            Amount = 1.0,
                            Unit = ""
                        }
                    },
                    Calories = 1.0,
                    Carbs = "",
                    Fat = "",
                    Protein = ""
                }
            };

            var food = JsonConvert.DeserializeAnonymousType(json, definition);

            return new FoodDetails
            {
                FoodId = food.Id,
                FoodName = food.Title,
                Description = food.Description,
                ImagePath = food.Image,
                Nutrition = new Nutrition
                {
                    Nutrients = food.Nutrition.Nutrients
                    .Select(x => new Nutrient
                    {
                        Amount = x.Amount,
                        Name = x.Name,
                        Unit = x.Unit,
                    })
                    .ToList(),
                    Calories = food.Nutrition.Calories,
                    Carbs = food.Nutrition.Carbs,
                    Fat = food.Nutrition.Fat,
                    Protein = food.Nutrition.Protein
                },
                ServingInformation = new Serving
                {
                    Number = food.Servings.Number,
                    Unit = food.Servings.Unit,
                    Size = food.Servings.Size
                }
            };
        }

        private FoodSearch JsonToFoodSearch(string json)
        {
            var definition = new
            {
                Products = new[]
                {
                    new 
                    { 
                        Id = 1,
                        Title = "",
                        Image = ""
                    }
                },
                Offset = 1,
                Number = 1,
                TotalProducts = 1
            };

            var foods = JsonConvert.DeserializeAnonymousType(json, definition);

            return new FoodSearch
            {
                Foods = foods.Products.Select(x => new Food
                {
                    FoodId = x.Id,
                    FoodName = x.Title,
                    ImagePath = x.Image
                })
                .ToList(),
                Page = foods.Offset,
                PageSize = foods.Number,
                TotalFoods = foods.TotalProducts,
            };
        }
    }
}
