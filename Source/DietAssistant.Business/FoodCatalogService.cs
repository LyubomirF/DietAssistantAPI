using DietAssistant.Business.Configuration;
using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Business.Helpers;
using DietAssistant.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

#pragma warning disable

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

            var data = JsonToFoodSearch(response.Content, requestModel);

            if (data is null)
                return Result
                    .CreateWithError<FoodSearch>(EvaluationTypes.Failed, "Unable to fetch results.");

            return Result.Create(data);
        }

        public Task<Result<FoodDetails>> GetFoodByIdAsync(String id, ServingRequest request)
        {
            var foodId = int.Parse(id.Substring(1));

            if (IsProduct(id))
                return GetDetailsForProduct(foodId, request);

            return GetDetailsForWholeFood(foodId, request);
        }

        public async Task<Result<IReadOnlyCollection<FoodDetails>>> GetFoodsAsync(IEnumerable<ManyFoodDetailsRequest> requests)
        {
            var result = new List<FoodDetails>();

            foreach (var request in requests)
            {
                var foodResponse = await GetFoodByIdAsync(request.FoodId, request.Serving);

                if (foodResponse.IsFailure())
                    return Result
                        .CreateWithError<IReadOnlyCollection<FoodDetails>>(EvaluationTypes.NotFound, "Food with id was not found");

                result.Add(foodResponse.Data);
            }

            return Result.Create<IReadOnlyCollection<FoodDetails>>(result);
        }

        private RestRequest GetSearchFoodsRestRequest(SearchFoodRequest request)
        {
            var restRequest = new RestRequest();

            restRequest
                .AddQueryParameter("offset", request.Page)
                .AddQueryParameter("number", request.PageSize)
                .AddQueryParameter("query", request.SearchQuery);

            if (request.FoodType == FoodType.Product)
            {
                restRequest.Resource = NutritionApiRoutes.SearchProducts;
            }
            else
            {
                restRequest.Resource = NutritionApiRoutes.SearchIngredients;
                restRequest
                    .AddQueryParameter("sort", "calories")
                    .AddQueryParameter("sortDirection", "asc");
            }

            return restRequest;
        }

        private FoodSearch JsonToFoodSearch(string json, SearchFoodRequest requesModel)
            => requesModel.FoodType switch
            {
                FoodType.Product => DeserializeJsonProducts(json),
                FoodType.WholeFood => DeserializeJsonIngredients(json),
                _ => null
            };

        private FoodSearch DeserializeJsonProducts(string json)
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
                    FoodId = GetProductId(x.Id),
                    FoodName = x.Title,
                    ImagePath = x.Image
                })
                .ToList(),
                Page = foods.Offset,
                PageSize = foods.Number,
                TotalFoods = foods.TotalProducts,
            };
        }

        private FoodSearch DeserializeJsonIngredients(string json)
        {
            var definition = new
            {
                Results = new[]
                {
                    new
                    {
                        Id = 1,
                        Name = "",
                        Image = ""
                    }
                },
                Offset = 1,
                Number = 1,
                TotalResults = 1
            };

            var foods = JsonConvert.DeserializeAnonymousType(json, definition);

            return new FoodSearch
            {
                Foods = foods.Results.Select(x => new Food
                {
                    FoodId = GetWholeFoodId(x.Id),
                    FoodName = x.Name,
                    ImagePath = x.Image
                })
                .ToList(),
                Page = foods.Offset,
                PageSize = foods.Number,
                TotalFoods = foods.TotalResults,
            };
        }

        private async Task<Result<FoodDetails>> GetDetailsForWholeFood(Int32 id, ServingRequest request)
        {
            var restRequest = new RestRequest(NutritionApiRoutes.GetIngredient(id));

            if (request != null && request.Amount.HasValue && request.Unit != null)
            {
                restRequest
                    .AddQueryParameter("amount", request.Amount.Value)
                    .AddQueryParameter("unit", request.Unit);
            }
            else
            {
                restRequest
                    .AddQueryParameter("amount", 100)
                    .AddQueryParameter("unit", "g");
            }

            var response = await _restClient.GetAsync(restRequest);

            if (response is null)
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.Failed, "Unable to fetch results.");

            var food = IngredientJsonToFoodDetails(response.Content);

            if (!IsUnitAllowed(food, request.Unit))
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.InvalidParameters, "Cannot convert to unit.");

            if (food is null)
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.Failed, "Unable to fetch results.");

            return Result.Create(food);
        }

        private async Task<Result<FoodDetails>> GetDetailsForProduct(Int32 id, ServingRequest request)
        {
            var restRequest = new RestRequest(NutritionApiRoutes.GetProduct(id));

            var response = await _restClient.GetAsync(restRequest);

            if (response is null)
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.Failed, "Unable to fetch results.");

            var data = ProductJsonToFoodDetails(response.Content);

            if (data is null)
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.Failed, "Unable to fetch results.");

            return CaculateFoodNutrition(data, request);
        }

        private Result<FoodDetails> CaculateFoodNutrition(FoodDetails food, ServingRequest request)
        {
            if (request != null && request.Unit == null && !request.Amount.HasValue)
                return Result.Create(food);

            if (!IsUnitAllowed(food, request.Unit))
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.InvalidParameters, "Cannot convert to unit.");

            var servingSize = food.ServingInformation.Size;
            var servingUnit = food.ServingInformation.Unit;

            return Result.Create(food.CalculateNutrition(servingSize, servingUnit, request.Amount.Value, request.Unit));
        }

        private FoodDetails ProductJsonToFoodDetails(string json)
        {
            var definition = new
            {
                Id = 1,
                Title = "",
                Description = "",
                Image = "",
                Servings = new
                {
                    Number = new Nullable<Double>(1.0),
                    Size = new Nullable<Double>(1.0),
                    Unit = ""
                },
                Nutrition = new
                {
                    Nutrients = new[]
                    {
                        new {
                            Name = "",
                            Amount = new Nullable<Double>(1.0),
                            Unit = ""
                        }
                    }
                }
            };

            var food = JsonConvert.DeserializeAnonymousType(json, definition,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include
                });

            return new FoodDetails
            {
                FoodId = GetProductId(food.Id),
                FoodName = food.Title,
                ImagePath = food.Image,
                Nutrition = new Nutrition
                {
                    Nutrients = food.Nutrition.Nutrients
                    .Select(x => new Nutrient
                    {
                        Amount = x.Amount.HasValue ? x.Amount.Value : 0,
                        Name = x.Name,
                        Unit = x.Unit,
                    })
                    .ToList(),
                },
                ServingInformation = new Serving
                {
                    Number = food.Servings.Number.HasValue ? food.Servings.Number.Value : 0,
                    Unit = food.Servings.Unit,
                    Size = food.Servings.Size.HasValue ? food.Servings.Size.Value : 0
                },
                PossibleUnits = food.Servings.Unit == "g" || food.Servings.Unit == "oz"
                    ? new List<string> { "g", "oz" }
                    : new List<string> { food.Servings.Unit }
            };
        }

        private FoodDetails IngredientJsonToFoodDetails(string json)
        {
            var definition = new
            {
                Id = 1,
                Name = "",
                Image = "",
                Amount = 1.0,
                UnitShort = "",
                Nutrition = new
                {
                    Nutrients = new[]
                    {
                        new {
                            Name = "",
                            Amount = new Nullable<Double>(1.0),
                            Unit = ""
                        }
                    },
                },
                PossibleUnits = new[] { "" }
            };

            var food = JsonConvert.DeserializeAnonymousType(json, definition,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include
                });

            return new FoodDetails
            {
                FoodId = GetProductId(food.Id),
                FoodName = food.Name,
                ImagePath = food.Image,
                Nutrition = new Nutrition
                {
                    Nutrients = food.Nutrition.Nutrients
                    .Select(x => new Nutrient
                    {
                        Amount = x.Amount.HasValue ? x.Amount.Value : 0,
                        Name = x.Name,
                        Unit = x.Unit,
                    })
                    .ToList(),
                },
                ServingInformation = new Serving
                {
                    Number = 1,
                    Unit = food.UnitShort,
                    Size = food.Amount
                },
                PossibleUnits = food.PossibleUnits.ToList()
            };
        }

        private String GetProductId(Int32 id) => "P" + id;

        private String GetWholeFoodId(Int32 id) => "W" + id;

        private Boolean IsProduct(String id) =>
            id.Substring(0, 1) == "P";

        private Boolean IsUnitAllowed(FoodDetails food, string unit)
            => food.PossibleUnits.Contains(unit);
    }
}
