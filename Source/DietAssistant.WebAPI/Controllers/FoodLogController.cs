using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    [Route("api/log")]
    [ApiController]
    public class FoodLogController : ControllerBase
    {
        private readonly IMealLogService _meallogService;

        public FoodLogController(IMealLogService foodLogService)
        {
            _meallogService = foodLogService;
        }

        [HttpPost("meal")]
        public async Task<IActionResult> LogMealAsync([FromBody] LogMealRequest request)
            => await _meallogService.LogNewMealAsync(request).ToActionResult(this);

        [HttpPost("food")]
        public async Task<IActionResult> LogFoodAsync(LogFoodRequest request)
            => await _meallogService.LogFoodAsync(request).ToActionResult(this);
    }
}
