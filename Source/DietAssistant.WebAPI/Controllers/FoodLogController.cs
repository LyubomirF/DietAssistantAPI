using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodLog.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    [Route("api/log")]
    [ApiController]
    public class FoodLogController : ControllerBase
    {
        private readonly IFoodLogService _foodLogService;

        public FoodLogController(IFoodLogService foodLogService)
        {
            _foodLogService = foodLogService;
        }

        [HttpPost("meal")]
        public async Task<IActionResult> LogMealAsync([FromBody] LogMealRequest request)
            => await _foodLogService.LogNewMealAsync(request).ToActionResult(this);

        [HttpPost("food")]
        public async Task<IActionResult> LogFoodAsync(LogFoodRequest request)
            => await _foodLogService.LogFoodAsync(request).ToActionResult(this);
    }
}
