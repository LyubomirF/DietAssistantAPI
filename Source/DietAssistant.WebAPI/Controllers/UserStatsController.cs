using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.UserStats.Requests;
using DietAssistant.Business.Contracts.Models.UserStats.Responses;
using DietAssistant.Common;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    using static UserStatsRoutes;

    [Route(Stats)]
    [Authorize]
    public class UserStatsController : BaseController
    {
        private readonly IUserStatsService _userStatsService;

        public UserStatsController(IUserStatsService userStatsService)
            => _userStatsService = userStatsService;

        /// <summary>
        /// Gets stats of user.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<UserStatsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserStats()
            => await _userStatsService.GetUserStatsAsync().ToActionResultAsync(this);

        /// <summary>
        /// Creates stats of user.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<UserStatsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetStatsAsync([FromBody] UserStatsRequest request)
            => await _userStatsService.SetUserStatsAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Change weight unit.
        /// </summary>
        [HttpPatch(WeightUnit)]
        [ProducesResponseType(typeof(Result<UserStatsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeWeightUnitAsync(ChangeWeightUnitRequest request)
            => await _userStatsService.ChangeWeightUnitAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Change height unit.
        /// </summary>
        [HttpPatch(HeightUnit)]
        [ProducesResponseType(typeof(Result<UserStatsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeHeightUnitAsync(ChangeHeightUnitRequest request)
            => await _userStatsService.ChangeHeightUnitAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Change weight.
        /// </summary>
        [HttpPatch(Weight)]
        [ProducesResponseType(typeof(Result<UserStatsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeWeightAsync(ChangeWeightRequest request)
            => await _userStatsService.ChangeWeightAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Change height.
        /// </summary>
        [HttpPatch(Height)]
        [ProducesResponseType(typeof(Result<UserStatsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeHeightAsync(ChangeHeightRequest request)
            => await _userStatsService.ChangeHeightAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Change gender.
        /// </summary>
        [HttpPatch(Gender)]
        [ProducesResponseType(typeof(Result<UserStatsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeGenderAsync(ChangeGenderRequest request)
            => await _userStatsService.ChangeGenderAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Change date of birth.
        /// </summary>
        [HttpPatch(DateOfBirth)]
        [ProducesResponseType(typeof(Result<UserStatsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeDateOfBirthAsync(ChangeDateOfBirthRequest request)
            => await _userStatsService.ChangeDateOfBirthAsync(request).ToActionResultAsync(this);
    } 
}
