using Newtonsoft.Json;

namespace DietAssistant.Business.NutritionApi.Responses
{
    public class SearchResponse
    {
        public List<Product> Products { get; set; }

        [JsonProperty("offset")]
        public Int32 Page { get; set; }

        [JsonProperty("number")]
        public Int32 PageSize { get; set; }

        public Int32 TotalProducts { get; set; }
    }
}
