using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    [Route("api/log")]
    [ApiController]
    public class MealLogController : ControllerBase
    {
        private readonly IMealLogService _mealLogService;

        public MealLogController(IMealLogService mealLogService)
            => _mealLogService = mealLogService;

        [HttpGet("meal/{mealId}")]
        public async Task<IActionResult> GetMealByIdAsync([FromRoute] Int32 mealId)
            => await _mealLogService.GetMealById(mealId).ToActionResult(this);

        [HttpPatch("meal/{mealId}")]
        public async Task<IActionResult> UpdateMealLogAsync([FromRoute] Int32 mealId, [FromBody] UpdateMealLogRequest request)
            => await _mealLogService.UpdateMealLogAsync(mealId, request).ToActionResult(this);

        [HttpDelete("meal/{mealId}")]
        public async Task<IActionResult> DeleteMealLogAsync([FromRoute] Int32 mealId)
            => await _mealLogService.DeleteMealAsync(mealId).ToActionResult(this);

        [HttpPost("meal")]
        public async Task<IActionResult> LogMealAsync([FromBody] LogMealRequest request)
            => await _mealLogService.LogMealAsync(request).ToActionResult(this);

        [HttpPost("meal/{mealId}/food")]
        public async Task<IActionResult> LogFoodAsync(Int32 mealId, LogFoodRequest request)
            => await _mealLogService.LogFoodAsync(mealId, request).ToActionResult(this);
    }
}
