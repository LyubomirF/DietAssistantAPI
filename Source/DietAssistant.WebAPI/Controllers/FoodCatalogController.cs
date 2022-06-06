using DietAssistant.Business;
using DietAssistant.Business.NutritionApi.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    [Route("api/food")]
    [ApiController]
    public class FoodCatalogController : ControllerBase
    {
        private readonly NutritionClient _nutritionClient;

        public FoodCatalogController(NutritionClient foodCatalog)
        {
            _nutritionClient = foodCatalog;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFoodsAsync([FromQuery] SearchFoodRequest request)
            => await _nutritionClient.SearchFoodsAsync(request).ToActionResult(this);

        // 758951
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFoodByIdAsync([FromRoute] Int32 id)
            => await _nutritionClient.GetFoodByIdAsync(id).ToActionResult(this);
    }
}
