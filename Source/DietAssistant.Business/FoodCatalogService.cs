using DietAssistant.Business.Configuration;
using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
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
            {
                return GetFoodDetails(NutritionApiRoutes.GetProduct(foodId), null, ProductJsonToFoodDetails);
            }

            var queryParams = new List<(string name, string value)>
            {
                ("amount", "100"),
                ("unit", "gram") 
            };

            return GetFoodDetails(NutritionApiRoutes.GetIngredient(foodId), queryParams, IngredientJsonToFoodDetails);
        }

        public async Task<Result<IReadOnlyCollection<FoodDetails>>> GetFoodsAsync(IEnumerable<String> foodIds)
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
                    }
                }
            };

            var food = JsonConvert.DeserializeAnonymousType(json, definition);

            if (food.Servings.Unit == "g" 
                || food.Servings.Unit == "gram" 
                || food.Servings.Unit == "oz"
                || food.Servings.Unit == "ounce")
            {

            }

            return new FoodDetails
            {
                FoodId =  GetProductId(food.Id),
                FoodName = food.Title,
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
                },
                ServingInformation = new Serving
                {
                    Number = food.Servings.Number,
                    Unit = food.Servings.Unit,
                    Size = food.Servings.Size
                },
                PossibleUnits = food.Servings.Unit == "g" || food.Servings.Unit == "oz"
                    ? new List<string> { "g", "oz"}
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
                            Amount = 1.0,
                            Unit = ""
                        }
                    },
                    WeightPerServing = new
                    {
                        Amount = 1.0,
                        Unit = ""
                    }
                },
                PossibleUnits = new[] { "" }
            };

            var food = JsonConvert.DeserializeAnonymousType(json, definition);

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
                        Amount = x.Amount,
                        Name = x.Name,
                        Unit = x.Unit,
                    })
                    .ToList(),
                },
                ServingInformation = new Serving
                {
                    Number = 1,
                    Unit = food.Nutrition.WeightPerServing.Unit,
                    Size = food.Nutrition.WeightPerServing.Amount
                },
                PossibleUnits = food.PossibleUnits.ToList()
            };
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
                TotallResults = 1
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
                TotalFoods = foods.TotallResults,
            };
        }

        private async Task<Result<FoodDetails>> GetFoodDetails(
            String url,
            List<(string name, string value)> parameters,
            Func<string, FoodDetails> converter)
        {
            var request = new RestRequest(url);

            if(parameters is not null)
            {
                foreach (var param in parameters)
                {
                    request.AddQueryParameter(param.name, param.value);
                }
            }

            var response = await _restClient.GetAsync(request);

            if (response is null)
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.Failed, "Unable to fetch results.");

            var data = converter(response.Content);

            if (data is null)
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.Failed, "Unable to fetch results.");

            return Result.Create(data);
        }

        private String GetProductId(Int32 id) => "P" + id;

        private String GetWholeFoodId(Int32 id) => "W" + id;

        private Boolean IsProduct(String id) =>
            id.Substring(0, 1) == "P";
    }
}
