using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodServing.Requests;
using DietAssistant.Business.Contracts.Models.FoodServing.Responses;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Responses;
using DietAssistant.Common;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable

namespace DietAssistant.WebAPI.Controllers
{
    using static DiaryRoutes;

    [Route(Diary)]
    [Authorize]
    public class DiaryController : BaseController
    {
        private readonly IMealService _mealService;
        private readonly IFoodServingService _foodServingService;

        public DiaryController(IMealService mealService, IFoodServingService foodServingService)
        {
            _mealService = mealService;
            _foodServingService = foodServingService;
        }

        /// <summary>
        /// Gets meals logged on a given day.
        /// </summary>
        [HttpGet(Meals)]
        [ProducesResponseType(typeof(Result<IEnumerable<MealLogResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMealsOnDate([FromQuery] DateTime? date)
            => await _mealService.GetMealsOnDateAsync(date).ToActionResultAsync(this);

        /// <summary>
        /// Gets consumed calories on a given day with comparison to goal.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet(Calories)]
        [ProducesResponseType(typeof(Result<DayCaloriesProgress>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCaloriesAsync([FromQuery] DateTime? date)
            => await _mealService.GetCaloriesBreakdownAsync(date).ToActionResultAsync(this);


        /// <summary>
        /// Gets macros for a given given day with comparison to goal.
        /// </summary>
        [HttpGet(Macros)]
        [ProducesResponseType(typeof(Result<DayMacrosProgress>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMacrosAsync([FromQuery] DateTime? date)
            => await _mealService.GetMacrosBreakdownAsync(date).ToActionResultAsync(this);

        /// <summary>
        ///  Gets a meal by id.
        /// </summary>
        [HttpGet(Meal)]
        [ProducesResponseType(typeof(Result<MealLogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMealByIdAsync([FromRoute] Int32 mealId)
            => await _mealService.GetMealById(mealId).ToActionResultAsync(this);

        /// <summary>
        /// Creates a meal log.
        /// </summary>
        [HttpPost(Meals)]
        [ProducesResponseType(typeof(Result<MealLogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LogMealAsync([FromBody] LogMealRequest request)
            => await _mealService.LogMealAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Updates a meal log.
        /// </summary>
        [HttpPatch(Meal)]
        [ProducesResponseType(typeof(Result<MealLogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMealLogAsync([FromRoute] Int32 mealId, [FromBody] UpdateMealLogRequest request)
            => await _mealService.UpdateMealLogAsync(mealId, request).ToActionResultAsync(this);

        /// <summary>
        /// Deletes a meal log.
        /// </summary>
        [HttpDelete(Meal)]
        [ProducesResponseType(typeof(Result<Int32>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMealLogAsync([FromRoute] Int32 mealId)
            => await _mealService.DeleteMealAsync(mealId).ToActionResultAsync(this);

        /// <summary>
        /// Adds a food serving log to a meal log.
        /// </summary>
        [HttpPost(FoodServings)]
        [ProducesResponseType(typeof(Result<FoodServingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LogFoodServingAsync([FromRoute] Int32 mealId,[FromBody] LogUpdateFoodServingRequest request)
            => await _foodServingService.LogFoodServingAsync(mealId, request).ToActionResultAsync(this);

        /// <summary>
        /// Updates a food serving log.
        /// </summary>
        [HttpPatch(FoodServing)]
        [ProducesResponseType(typeof(Result<FoodServingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFoodServingLogAsync(
            [FromRoute] Int32 mealId,
            [FromRoute] Int32 foodServingId,
            [FromBody] LogUpdateFoodServingRequest request)
            => await _foodServingService.UpdateFoodServingLogAsync(mealId, foodServingId, request).ToActionResultAsync(this);

        /// <summary>
        /// Deletes a food serving log.
        /// </summary>
        [HttpDelete(FoodServing)]
        [ProducesResponseType(typeof(Result<Int32>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFoodLogAsync([FromRoute] Int32 mealId, [FromRoute] Int32 foodServingId)
            => await _foodServingService.DeleteFoodServingLogAsync(mealId, foodServingId).ToActionResultAsync(this);
    }
}
