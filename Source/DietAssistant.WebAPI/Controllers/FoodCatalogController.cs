﻿using DietAssistant.Business;
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchFoodsAsync([FromQuery] SearchFoodRequest request)
        {
            var response = await _nutritionClient.SearchFoodsAsync(request);

            return Ok(response);
        }
        // 758951
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFoodByIdAsync([FromRoute] Int32 id)
        {
            var response = await _nutritionClient.GetFoodByIdAsync(id);

            return Ok(response);
        }
    }
}
