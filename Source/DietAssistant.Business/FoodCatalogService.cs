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

            if (!response.IsSuccessful)
                return Result
                    .CreateWithError<FoodSearch>(EvaluationTypes.InvalidParameters, response.ErrorMessage);

            var data = JsonToFoodSearch(response.Content, requestModel);

            if (data is null)
                return Result
                    .CreateWithError<FoodSearch>(EvaluationTypes.Failed, ResponseMessages.ActionFailed);

            return Result.Create(data);
        }

        public Task<Result<FoodDetails>> GetFoodByIdAsync(String id, ServingRequest request)
        {
            var foodId = int.Parse(id.Substring(1));

            return IsProduct(id) 
                ? GetDetailsForProduct(foodId, request) 
                : GetDetailsForWholeFood(foodId, request);
        }

        public async Task<Result<IReadOnlyCollection<FoodDetails>>> GetFoodsAsync(IEnumerable<ManyFoodDetailsRequest> requests)
        {
            var result = new List<FoodDetails>();

            foreach (var request in requests)
            {
                var foodResponse = await GetFoodByIdAsync(request.FoodId, request.Serving);

                if (foodResponse.IsFailure())
                    return Result
                        .CreateWithErrors<IReadOnlyCollection<FoodDetails>>(EvaluationTypes.NotFound, foodResponse.Errors);

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

        private FoodSearch? JsonToFoodSearch(string json, SearchFoodRequest requesModel)
            => requesModel.FoodType switch
            {
                FoodType.Product => FoodJsonConverter.DeserializeJsonProducts(json),
                FoodType.WholeFood => FoodJsonConverter.DeserializeJsonWholeFoods(json),
                _ => null
            };

        private async Task<Result<FoodDetails>> GetDetailsForProduct(Int32 id, ServingRequest request)
        {
            var restRequest = new RestRequest(NutritionApiRoutes.GetProduct(id));

            var response = await _restClient.GetAsync(restRequest);

            if (!response.IsSuccessful)
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.Failed, response.ErrorMessage);

            var data = FoodJsonConverter.ParseProductJson(response.Content);

            if (data is null)
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.Failed, ResponseMessages.ActionFailed);

            return CaculateFoodNutrition(data, request);
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

            if (!response.IsSuccessful)
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.Failed, response.ErrorMessage);

            var food = FoodJsonConverter.ParseWholeFoodJson(response.Content);

            if (request != null && request.Unit != null && request.Amount.HasValue && !food.IsUnitAllowed(request.Unit))
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.InvalidParameters, ResponseMessages.CannotConvertToUnit);

            if (food is null)
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.Failed, ResponseMessages.ActionFailed);

            return Result.Create(food);
        }

        private Result<FoodDetails> CaculateFoodNutrition(FoodDetails food, ServingRequest request)
        {
            if (request is null || (string.IsNullOrEmpty(request.Unit) && !request.Amount.HasValue))
                return Result.Create(food);

            if (!food.IsUnitAllowed(request.Unit))
                return Result
                    .CreateWithError<FoodDetails>(EvaluationTypes.InvalidParameters, "Cannot convert to unit.");

            var servingSize = food.ServingInformation.Size;
            var servingUnit = food.ServingInformation.Unit;

            return Result.Create(food.CalculateNutrition(servingSize, servingUnit, request.Amount.Value, request.Unit));
        }

        private Boolean IsProduct(String id) =>
            id.Substring(0, 1) == "P";
    }
}
