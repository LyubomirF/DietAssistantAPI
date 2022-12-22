using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Common;
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

        /// <summary>
        /// Gets a list of foods.
        /// </summary>
        [HttpGet(SearchFood)]
        [ProducesResponseType(typeof(Result<FoodSearch>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchFoodsAsync([FromQuery] SearchFoodRequest request)
            => await _foodCatalog.SearchFoodsAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Gets food by id.
        /// </summary>
        [HttpGet(Food)]
        [ProducesResponseType(typeof(Result<FoodDetails>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFoodByIdAsync([FromRoute] String id, [FromQuery] ServingRequest request)
            => await _foodCatalog.GetFoodByIdAsync(id, request).ToActionResultAsync(this);
    }
}
