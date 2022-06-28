﻿using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodServing.Requests;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    using static DiaryRoutes;

    [Route("api/" + Diary)]
    [ApiController]
    [Authorize]
    public class DiaryController : ControllerBase
    {
        private readonly IMealService _mealService;
        private readonly IFoodServingService _foodServingService;

        public DiaryController(IMealService mealService, IFoodServingService foodServingService)
        {
            _mealService = mealService;
            _foodServingService = foodServingService;
        }

        [HttpGet(Meals)]
        public async Task<IActionResult> GetMealsOnDate([FromQuery] DateTime? date)
            => await _mealService.GetMealsOnDateAsync(date).ToActionResult(this);

        [HttpGet(Meal)]
        public async Task<IActionResult> GetMealByIdAsync([FromRoute] Int32 mealId)
            => await _mealService.GetMealById(mealId).ToActionResult(this);

        [HttpPost(Meals)]
        public async Task<IActionResult> LogMealAsync([FromBody] LogMealRequest request)
            => await _mealService.LogMealAsync(request).ToActionResult(this);

        [HttpPatch(Meal)]
        public async Task<IActionResult> UpdateMealLogAsync([FromRoute] Int32 mealId, [FromBody] UpdateMealLogRequest request)
            => await _mealService.UpdateMealLogAsync(mealId, request).ToActionResult(this);

        [HttpDelete(Meal)]
        public async Task<IActionResult> DeleteMealLogAsync([FromRoute] Int32 mealId)
            => await _mealService.DeleteMealAsync(mealId).ToActionResult(this);

        [HttpPost(FoodServings)]
        public async Task<IActionResult> LogFoodServingAsync([FromRoute] Int32 mealId,[FromBody] LogUpdateFoodServingRequest request)
            => await _foodServingService.LogFoodServingAsync(mealId, request).ToActionResult(this);

        [HttpPatch(FoodServing)]
        public async Task<IActionResult> UpdateFoodServingLogAsync(
            [FromRoute] Int32 mealId,
            [FromRoute] Int32 foodServingId,
            [FromBody] LogUpdateFoodServingRequest request)
            => await _foodServingService.UpdateFoodServingLogAsync(mealId, foodServingId, request).ToActionResult(this);

        [HttpDelete(FoodServing)]
        public async Task<IActionResult> DeleteFoodLogAsync([FromRoute] Int32 mealId, [FromRoute] Int32 foodServingId)
            => await _foodServingService.DeleteFoodServingLogAsync(mealId, foodServingId).ToActionResult(this);
    }
}
