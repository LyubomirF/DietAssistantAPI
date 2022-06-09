using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    [Route("api/food")]
    [ApiController]
    public class FoodCatalogController : ControllerBase
    {
        private readonly IFoodCatalogService _foodCatalog;

        public FoodCatalogController(IFoodCatalogService foodCatalog)
        {
            _foodCatalog = foodCatalog;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFoodsAsync([FromQuery] SearchFoodRequest request)
            => await _foodCatalog.SearchFoodsAsync(request).ToActionResult(this);

        // 758951
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFoodByIdAsync([FromRoute] Int32 id)
            => await _foodCatalog.GetFoodByIdAsync(id).ToActionResult(this);
    }
}
