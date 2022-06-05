#pragma warning disable CS8618

using Newtonsoft.Json;

namespace DietAssistant.Business.NutritionApi.Responses
{
    public class Product
    {
        [JsonProperty("id")]
        public Int32 ProductId { get; set; }

        [JsonProperty("title")]
        public String ProductName { get; set; }

        [JsonProperty("image")]
        public String ImagePath { get; set; }
    }
}
