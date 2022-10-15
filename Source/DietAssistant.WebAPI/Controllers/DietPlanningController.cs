using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.DietPlanning.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    using static DietPlanningRoutes;

    [Route(DietPlans)]
    [Authorize]
    public class DietPlanningController : BaseController
    {
        private readonly IDietPlanningService _dietPlanningService;

        public DietPlanningController(IDietPlanningService dietPlanningService)
            => _dietPlanningService = dietPlanningService;

        [HttpGet(Macros)]
        public async Task<IActionResult> GetDietPlanMacrosAsync([FromRoute] Int32 dietPlanId)
            => await _dietPlanningService.GetDietPlanMacrosAsync(dietPlanId).ToActionResultAsync(this);

        [HttpGet(DietPlan)]
        public async Task<IActionResult> GetDietPlanAsync([FromRoute] Int32 dietPlanId)
            => await _dietPlanningService.GetDietPlanAsync(dietPlanId).ToActionResultAsync(this);

        [HttpPost]
        public async Task<IActionResult> CreateDietPlanAsync([FromBody] CreateDietPlanRequest request)
            => await _dietPlanningService.CreateDietPlanAsync(request).ToActionResultAsync(this);

        [HttpDelete(DietPlan)]
        public async Task<IActionResult> DeleteDietPlanAsync([FromRoute] Int32 dietPlanId)
            => await _dietPlanningService.DeleteDietPlanAsync(dietPlanId).ToActionResultAsync(this);

        [HttpPost(MealPlans)]
        public async Task<IActionResult> AddMealPlanAsync([FromRoute] Int32 dietPlanId, [FromBody] AddUpdateMealRequest request)
            => await _dietPlanningService.AddMealPlanAsync(dietPlanId, request).ToActionResultAsync(this);

        [HttpPut(MealPlan)]
        public async Task<IActionResult> UpdateMealPlanAsync(
            [FromRoute] Int32 dietPlanId,
            [FromRoute] Int32 mealPlanId,
            [FromBody] AddUpdateMealRequest request)
            => await _dietPlanningService.UpdateMealPlanAsync(dietPlanId, mealPlanId, request).ToActionResultAsync(this);

        [HttpDelete(MealPlan)]
        public async Task<IActionResult> DeleteMealPlanAsync([FromRoute] Int32 dietPlanId,[FromRoute] Int32 mealPlanId)
            => await _dietPlanningService.DeleteMealPlanAsync(dietPlanId, mealPlanId).ToActionResultAsync(this);

        [HttpPost(FoodPlans)]
        public async Task<IActionResult> AddFoodPlanAsync(
            [FromRoute] Int32 dietPlanId,
            [FromRoute] Int32 mealPlanId,
            [FromBody] FoodPlanRequest request)
            => await _dietPlanningService.AddFoodPlanAsync(dietPlanId, mealPlanId, request).ToActionResultAsync(this);

        [HttpPut(FoodPlan)]
        public async Task<IActionResult> UpdateFoodPlanAsync(
            [FromRoute] Int32 dietPlanId,
            [FromRoute] Int32 mealPlanId,
            [FromRoute] Int32 foodPlanId,
            [FromBody] FoodPlanRequest request)
            => await _dietPlanningService.UpdateFoodPlanAsync(dietPlanId, mealPlanId, foodPlanId, request).ToActionResultAsync(this);

        [HttpDelete(FoodPlan)]
        public async Task<IActionResult> DeleteFoodPlan(
            [FromRoute] Int32 dietPlanId,
            [FromRoute] Int32 mealPlanId,
            [FromRoute] Int32 foodPlanId)
            => await _dietPlanningService.DeleteFoodPlanAsync(dietPlanId, mealPlanId, foodPlanId).ToActionResultAsync(this);
    }
}
