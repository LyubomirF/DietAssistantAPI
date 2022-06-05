using DietAssistant.Business.NutritionApi.Requests;
using DietAssistant.Business.NutritionApi.Responses;
using Microsoft.Extensions.Options;
using RestSharp;
using Newtonsoft.Json;
using DietAssistant.Common;

namespace DietAssistant.Business
{
    public class NutritionClient
    {
        private readonly RestClient _restClient;
        private readonly NutritionApiConfiguration _apiConfiguration;

        public NutritionClient(IOptions<NutritionApiConfiguration> options)
        {
            _apiConfiguration = options.Value;
            _restClient = new RestClient(NutritionApiRoutes.BaseUrl)
                .AddDefaultHeader("X-RapidAPI-Host", _apiConfiguration.Host)
                .AddDefaultHeader("X-RapidAPI-Key", _apiConfiguration.Key);
        }

        public async Task<Result<SearchResponse>> SearchFoods(SearchFoodRequest requestModel)
        {
            var request = GetSearchFoodsRestRequest(requestModel);

            var response = await _restClient.GetAsync(request);

            if (response == null)
                return Result.Create<SearchResponse>(EvaluationTypes.Failed, "Unable to fetch results.");

            var data = JsonConvert.DeserializeObject<SearchResponse>(response.Content);

            if (data == null)
                return Result.Create<SearchResponse>(EvaluationTypes.Failed, "Unable to fetch results.");

            return Result.Create(data);
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
    }
}
