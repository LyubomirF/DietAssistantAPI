using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Goal.Requests;
using DietAssistant.Business.Contracts.Models.Goal.Responses;
using DietAssistant.Common;
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

        /// <summary>
        /// Gets goal of user.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGoalAsync()
             => await _goalService.GetGoalAsync().ToActionResultAsync(this);

        /// <summary>
        /// Updates current weight.
        /// </summary>
        [HttpPatch(CurrentWeight)]
        [ProducesResponseType(typeof(Result<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeCurrentWeightAsync([FromBody] ChangeCurrentWeighRequest request)
            => await _goalService.ChangeCurrentWeightAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Updates goal weight.
        /// </summary>
        [HttpPatch(GoalWeight)]
        [ProducesResponseType(typeof(Result<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeGoalWeightAsync([FromBody] ChangeGoalWeightRequest request)
            => await _goalService.ChangeGoalWeightAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Updates weekly goal.
        /// </summary>
        [HttpPatch(WeeklyGoal)]
        [ProducesResponseType(typeof(Result<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeWeeklyGoalAsync([FromBody] ChangeWeeklyGoalRequest request)
            => await _goalService.ChangeWeeklyGoalAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Updates activity level.
        /// </summary>
        [HttpPatch(ActivityLevel)]
        [ProducesResponseType(typeof(Result<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeActivityLevelAsync([FromBody] ChangeActivityLevelRequest request)
            => await _goalService.ChangeActivityLevelAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Updates nutrition goal.
        /// </summary>
        [HttpPatch(NutritionGoal)]
        [ProducesResponseType(typeof(Result<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeNutritionGoalAsync([FromBody] NutritionGoalRequest request)
            => await _goalService.ChangeNutritionGoalAsync(request).ToActionResultAsync(this);
    }
}
