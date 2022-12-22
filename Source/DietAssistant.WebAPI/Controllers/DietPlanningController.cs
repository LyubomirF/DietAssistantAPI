using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.DietPlanning.Requests;
using DietAssistant.Business.Contracts.Models.DietPlanning.Responses;
using DietAssistant.Business.Contracts.Models.DietPlanning.Responses.Macros;
using DietAssistant.Common;
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

        /// <summary>
        /// Gets macros of a diet plan.
        /// </summary>
        [HttpGet(Macros)]
        [ProducesResponseType(typeof(Result<DietPlanMacrosBreakdownResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDietPlanMacrosAsync([FromRoute] Int32 dietPlanId)
            => await _dietPlanningService.GetDietPlanMacrosAsync(dietPlanId).ToActionResultAsync(this);
        /// <summary>
        /// Gets a diet plan.
        /// </summary>
        [HttpGet(DietPlan)]
        [ProducesResponseType(typeof(Result<DietPlanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDietPlanAsync([FromRoute] Int32 dietPlanId)
            => await _dietPlanningService.GetDietPlanAsync(dietPlanId).ToActionResultAsync(this);

        /// <summary>
        /// Creates a diet plan.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<Int32>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateDietPlanAsync([FromBody] CreateDietPlanRequest request)
            => await _dietPlanningService.CreateDietPlanAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Deletes a diet plan.
        /// </summary>
        [HttpDelete(DietPlan)]
        [ProducesResponseType(typeof(Result<Int32>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDietPlanAsync([FromRoute] Int32 dietPlanId)
            => await _dietPlanningService.DeleteDietPlanAsync(dietPlanId).ToActionResultAsync(this);

        /// <summary>
        /// Adds meal plan to a diet plan.
        /// </summary>
        [HttpPost(MealPlans)]
        [ProducesResponseType(typeof(Result<MealPlanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddMealPlanAsync([FromRoute] Int32 dietPlanId, [FromBody] AddUpdateMealRequest request)
            => await _dietPlanningService.AddMealPlanAsync(dietPlanId, request).ToActionResultAsync(this);

        /// <summary>
        /// Updates a meal plan.
        /// </summary>
        [HttpPut(MealPlan)]
        [ProducesResponseType(typeof(Result<MealPlanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMealPlanAsync(
            [FromRoute] Int32 dietPlanId,
            [FromRoute] Int32 mealPlanId,
            [FromBody] AddUpdateMealRequest request)
            => await _dietPlanningService.UpdateMealPlanAsync(dietPlanId, mealPlanId, request).ToActionResultAsync(this);

        /// <summary>
        /// Deletes a meal plan
        /// </summary>
        [HttpDelete(MealPlan)]
        [ProducesResponseType(typeof(Result<Int32>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMealPlanAsync([FromRoute] Int32 dietPlanId,[FromRoute] Int32 mealPlanId)
            => await _dietPlanningService.DeleteMealPlanAsync(dietPlanId, mealPlanId).ToActionResultAsync(this);

        /// <summary>
        /// Adds food plan.
        /// </summary>
        [HttpPost(FoodPlans)]
        [ProducesResponseType(typeof(Result<MealPlanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddFoodPlanAsync(
            [FromRoute] Int32 dietPlanId,
            [FromRoute] Int32 mealPlanId,
            [FromBody] FoodPlanRequest request)
            => await _dietPlanningService.AddFoodPlanAsync(dietPlanId, mealPlanId, request).ToActionResultAsync(this);

        /// <summary>
        /// Updates food plan.
        /// </summary>
        [HttpPut(FoodPlan)]
        [ProducesResponseType(typeof(Result<MealPlanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFoodPlanAsync(
            [FromRoute] Int32 dietPlanId,
            [FromRoute] Int32 mealPlanId,
            [FromRoute] Int32 foodPlanId,
            [FromBody] FoodPlanRequest request)
            => await _dietPlanningService.UpdateFoodPlanAsync(dietPlanId, mealPlanId, foodPlanId, request).ToActionResultAsync(this);

        /// <summary>
        /// Deletes food plan.
        /// </summary>
        [HttpDelete(FoodPlan)]
        [ProducesResponseType(typeof(Result<MealPlanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFoodPlan(
            [FromRoute] Int32 dietPlanId,
            [FromRoute] Int32 mealPlanId,
            [FromRoute] Int32 foodPlanId)
            => await _dietPlanningService.DeleteFoodPlanAsync(dietPlanId, mealPlanId, foodPlanId).ToActionResultAsync(this);
    }
}
