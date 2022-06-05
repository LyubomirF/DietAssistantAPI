using Newtonsoft.Json;

namespace DietAssistant.Business.NutritionApi.Responses.Food
{
    public class FoodResponse
    {
        [JsonProperty("id")]
        public Int32 FoodId { get; set; }

        [JsonProperty("title")]
        public String FoodName { get; set; }

        [JsonProperty("nutrition")]
        public Nutrition Nutrition { get; set; }

        [JsonProperty("servings")]
        public ServingInformation ServingInformation { get; set; }

        [JsonProperty("description")]
        public String Description { get; set; }

        [JsonProperty("image")]
        public String ImagePath { get; set; }
    }
}
