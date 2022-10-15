using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    using static FoodCatalogRoutes;

    [Route(Foods)]
    public class FoodCatalogController : BaseController
    {
        private readonly IFoodCatalogService _foodCatalog;

        public FoodCatalogController(IFoodCatalogService foodCatalog)
        {
            _foodCatalog = foodCatalog;
        }

        [HttpGet(SearchFood)]
        public async Task<IActionResult> SearchFoodsAsync([FromQuery] SearchFoodRequest request)
            => await _foodCatalog.SearchFoodsAsync(request).ToActionResultAsync(this);

        // 758951 - cucumber
        // 186891 - chicken
        [HttpGet(Food)]
        public async Task<IActionResult> GetFoodByIdAsync([FromRoute] String id, [FromQuery] ServingRequest request)
            => await _foodCatalog.GetFoodByIdAsync(id, request).ToActionResultAsync(this);
    }
}
