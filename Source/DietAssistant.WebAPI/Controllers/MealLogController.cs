using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    [Route("api/log")]
    [ApiController]
    [Authorize]
    public class MealLogController : ControllerBase
    {
        private readonly IMealLogService _mealLogService;

        public MealLogController(IMealLogService mealLogService)
            => _mealLogService = mealLogService;

        [HttpGet("meal/{mealId}")]
        public async Task<IActionResult> GetMealByIdAsync([FromRoute] Int32 mealId)
            => await _mealLogService.GetMealById(mealId).ToActionResult(this);

        [HttpPost("meal")]
        public async Task<IActionResult> LogMealAsync([FromBody] LogMealRequest request)
            => await _mealLogService.LogMealAsync(request).ToActionResult(this);

        [HttpPatch("meal/{mealId}")]
        public async Task<IActionResult> UpdateMealLogAsync([FromRoute] Int32 mealId, [FromBody] UpdateMealLogRequest request)
            => await _mealLogService.UpdateMealLogAsync(mealId, request).ToActionResult(this);

        [HttpDelete("meal/{mealId}")]
        public async Task<IActionResult> DeleteMealLogAsync([FromRoute] Int32 mealId)
            => await _mealLogService.DeleteMealAsync(mealId).ToActionResult(this);

        [HttpPost("meal/{mealId}/food")]
        public async Task<IActionResult> LogFoodAsync([FromRoute] Int32 mealId,[FromBody] LogFoodRequest request)
            => await _mealLogService.LogFoodAsync(mealId, request).ToActionResult(this);

        [HttpPatch("meal/{mealId}/food-serving/{foodServingId}")]
        public async Task<IActionResult> UpdateFoodLogAsync(
            [FromRoute] Int32 mealId,
            [FromRoute] Int32 foodServingId,
            [FromBody] UpdateFoodLogRequest request)
            => await _mealLogService.UpdateFoodLogAsync(mealId, foodServingId, request).ToActionResult(this);

        [HttpDelete("meal/{mealId}/food-serving/{foodServingId}")]
        public async Task<IActionResult> DeleteFoodLogAsync([FromRoute] Int32 mealId, [FromRoute] Int32 foodServingId)
            => await _mealLogService.DeleteFoodLogAsync(mealId, foodServingId).ToActionResult(this);
    }
}
