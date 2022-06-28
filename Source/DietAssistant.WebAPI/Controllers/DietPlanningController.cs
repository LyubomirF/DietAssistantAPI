using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.DietPlanning.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    [Route("api/diet-plans")]
    [ApiController]
    [Authorize]
    public class DietPlanningController : ControllerBase
    {
        private readonly IDietPlanningService _dietPlanningService;

        public DietPlanningController(IDietPlanningService dietPlanningService)
            => _dietPlanningService = dietPlanningService;

        [HttpGet("{dietPlanId}/macros")]
        public async Task<IActionResult> GetDietPlanMacrosAsync([FromRoute] Int32 dietPlanId)
            => await _dietPlanningService.GetDietPlanMacrosAsync(dietPlanId).ToActionResult(this);

        [HttpGet("{dietPlanId}")]
        public async Task<IActionResult> GetDietPlanAsync([FromRoute] Int32 dietPlanId)
            => await _dietPlanningService.GetDietPlanAsync(dietPlanId).ToActionResult(this);

        [HttpPost]
        public async Task<IActionResult> CreateDietPlanAsync([FromBody] String planName)
            => await _dietPlanningService.CreateDietPlanAsync(planName).ToActionResult(this);

        [HttpDelete("{dietPlanId}")]
        public async Task<IActionResult> DeleteDietPlanAsync([FromRoute] Int32 dietPlanId)
            => await _dietPlanningService.DeleteDietPlanAsync(dietPlanId).ToActionResult(this);

        [HttpPost("{dietPlanId}/meal-plans")]
        public async Task<IActionResult> AddMealPlanAsync([FromRoute] Int32 dietPlanId, [FromBody] AddUpdateMealRequest request)
            => await _dietPlanningService.AddMealPlanAsync(dietPlanId, request).ToActionResult(this);

        [HttpPut("{dietPlanId}/meal-plans/{mealPlanId}")]
        public async Task<IActionResult> AddMealPlanAsync(
            [FromRoute] Int32 dietPlanId,
            [FromRoute] Int32 mealPlanId,
            [FromBody] AddUpdateMealRequest request)
            => await _dietPlanningService.UpdateMealPlanAsync(dietPlanId, mealPlanId, request).ToActionResult(this);

        [HttpDelete("{dietPlanId}/meal-plans/{mealPlanId}")]
        public async Task<IActionResult> DeleteMealPlanAsync([FromRoute] Int32 dietPlanId,[FromRoute] Int32 mealPlanId)
            => await _dietPlanningService.DeleteMealPlanAsync(dietPlanId, mealPlanId).ToActionResult(this);

        [HttpPost("{dietPlanId}/meal-plans/{mealPlanId}")]
        public async Task<IActionResult> AddFoodPlanAsync(
            [FromRoute] Int32 dietPlanId,
            [FromRoute] Int32 mealPlanId,
            [FromBody] FoodPlanRequest request)
            => await _dietPlanningService.AddFoodPlanAsync(dietPlanId, mealPlanId, request).ToActionResult(this);

        [HttpPost("{dietPlanId}/meal-plans/{mealPlanId}/food-plans/{foodPlanId}")]
        public async Task<IActionResult> UpdateFoodPlanAsync(
            [FromRoute] Int32 dietPlanId,
            [FromRoute] Int32 mealPlanId,
            [FromRoute] Int32 foodPlanId,
            [FromBody] FoodPlanRequest request)
            => await _dietPlanningService.UpdateFoodPlanAsync(dietPlanId, mealPlanId, foodPlanId, request).ToActionResult(this);

        [HttpDelete("{dietPlanId}/meal-plans/{mealPlanId}/food-plans/{foodPlanId}")]
        public async Task<IActionResult> DeleteFoodPlan(
            [FromRoute] Int32 dietPlanId,
            [FromRoute] Int32 mealPlanId,
            [FromRoute] Int32 foodPlanId)
            => await _dietPlanningService.DeleteFoodPlanAsync(dietPlanId, mealPlanId, foodPlanId).ToActionResult(this);
    }
}
