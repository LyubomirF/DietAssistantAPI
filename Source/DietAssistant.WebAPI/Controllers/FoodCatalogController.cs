using DietAssistant.Business;
using DietAssistant.Business.NutritionApi.Requests;
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

        [HttpGet]
        public async Task<IActionResult> GetSnickers([FromQuery] SearchFoodRequest request)
        {
            var response = await _nutritionClient.SearchFoods(request);

            return Ok(response);
        }
    }
}
