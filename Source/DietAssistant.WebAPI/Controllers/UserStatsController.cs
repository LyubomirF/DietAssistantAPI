using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.UserStats.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    [Route("api/stats")]
    [ApiController]
    [Authorize]
    public class UserStatsController : ControllerBase
    {
        private readonly IUserStatsService _userStatsService;

        public UserStatsController(IUserStatsService userStatsService)
            => _userStatsService = userStatsService;

        [HttpPost]
        public async Task<IActionResult> SetStatsAsync([FromBody] UserStatsRequest request)
            => await _userStatsService.SetUserStatsAsync(request).ToActionResult(this);
    }
}
