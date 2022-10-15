using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.UserStats.Requests;
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

        [HttpGet]
        public async Task<IActionResult> GetUserStats()
            => await _userStatsService.GetUserStats().ToActionResultAsync(this);

        [HttpPost]
        public async Task<IActionResult> SetStatsAsync([FromBody] UserStatsRequest request)
            => await _userStatsService.SetUserStatsAsync(request).ToActionResultAsync(this);

        [HttpPatch(WeightUnit)]
        public async Task<IActionResult> ChangeWeightUnitAsync(ChangeWeightUnitRequest request)
            => await _userStatsService.ChangeWeightUnitAsync(request).ToActionResultAsync(this);

        [HttpPatch(HeightUnit)]
        public async Task<IActionResult> ChangeHeightUnitAsync(ChangeHeightUnitRequest request)
            => await _userStatsService.ChangeHeightUnitAsync(request).ToActionResultAsync(this);

        [HttpPatch(Weight)]
        public async Task<IActionResult> ChangeWeightAsync(ChangeWeightRequest request)
            => await _userStatsService.ChangeWeightAsync(request).ToActionResultAsync(this);

        [HttpPatch(Height)]
        public async Task<IActionResult> ChangeHeightAsync(ChangeHeightRequest request)
            => await _userStatsService.ChangeHeightAsync(request).ToActionResultAsync(this);

        [HttpPatch(Gender)]
        public async Task<IActionResult> ChangeGenderAsync(ChangeGenderRequest request)
            => await _userStatsService.ChangeGenderAsync(request).ToActionResultAsync(this);

        [HttpPatch(DateOfBirth)]
        public async Task<IActionResult> ChangeDateOfBirthAsync(ChangeDateOfBirthRequest request)
            => await _userStatsService.ChangeDateOfBirthAsync(request).ToActionResultAsync(this);
    } 
}
