using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Goal.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    using static GoalRoutes;

    [Route(Goals)]
    [Authorize]
    public class GoalController : BaseController
    {
        private readonly IGoalService _goalService;

        public GoalController(IGoalService goalService)
            => _goalService = goalService;

        [HttpGet]
        public async Task<IActionResult> GetGoalAsync()
             => await _goalService.GetGoalAsync().ToActionResult(this);

        [HttpPatch(CurrentWeight)]
        public async Task<IActionResult> ChangeCurrentWeightAsync([FromBody] ChangeCurrentWeighRequest request)
            => await _goalService.ChangeCurrentWeightAsync(request).ToActionResult(this);

        [HttpPatch(GoalWeight)]
        public async Task<IActionResult> ChangeGoalWeightAsync([FromBody] ChangeGoalWeightRequest request)
            => await _goalService.ChangeGoalWeightAsync(request).ToActionResult(this);

        [HttpPatch(WeeklyGoal)]
        public async Task<IActionResult> ChangeWeeklyGoalAsync([FromBody] ChangeWeeklyGoalRequest request)
            => await _goalService.ChangeWeeklyGoalAsync(request).ToActionResult(this);

        [HttpPatch(ActivityLevel)]
        public async Task<IActionResult> ChangeActivityLevelAsync([FromBody] ChangeActivityLevelRequest request)
            => await _goalService.ChangeActivityLevelAsync(request).ToActionResult(this);

        [HttpPatch(NutritionGoal)]
        public async Task<IActionResult> ChangeNutritionGoalAsync([FromBody] NutritionGoalRequest request)
            => await _goalService.ChangeNutritionGoalAsync(request).ToActionResult(this);
    }
}
