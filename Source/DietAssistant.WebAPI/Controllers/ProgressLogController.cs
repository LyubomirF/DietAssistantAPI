using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.ProgressLog.Requests;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    using static ProgressLogRoutes;

    [Route("api/" + ProgressLogs)]
    [ApiController]
    [Authorize]
    public class ProgressLogController : ControllerBase
    {
        private readonly IProgressLogService _progressLogService;

        public ProgressLogController(IProgressLogService progressLogService)
            => _progressLogService = progressLogService;

        [HttpGet]
        public async Task<IActionResult> GetProgressLogsPagedAsync([FromQuery] ProgressLogFilterRequest request)
            => await _progressLogService.GetProgressLogsPagedAsync(request).ToActionResult(this);

        [HttpPost]
        public async Task<IActionResult> AddProgressLogAsync([FromBody] AddProgressLogRequest request)
            => await _progressLogService.AddProgressLogAsync(request).ToActionResult(this);

        [HttpDelete(ProgressLog)]
        public async Task<IActionResult> DeleteProgressLogAsync([FromRoute] Int32 progressLogId)
            => await _progressLogService.DeleteProgressLogAsync(progressLogId).ToActionResult(this);
    }
}
