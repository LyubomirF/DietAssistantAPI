using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Paging;
using DietAssistant.Business.Contracts.Models.ProgressLog.Requests;
using DietAssistant.Business.Contracts.Models.ProgressLog.Responses;
using DietAssistant.Common;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    using static ProgressLogRoutes;

    [Route(ProgressLogs)]
    [Authorize]
    public class ProgressLogController : BaseController
    {
        private readonly IProgressLogService _progressLogService;

        public ProgressLogController(IProgressLogService progressLogService)
            => _progressLogService = progressLogService;

        /// <summary>
        /// Gets progress logs paged with filter.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<Result<PagedResult<ProgressLogResponse>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProgressLogsPagedAsync([FromQuery] ProgressLogFilterRequest request)
            => await _progressLogService.GetProgressLogsPagedAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Creates a progress log.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<ProgressLogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddProgressLogAsync([FromBody] AddProgressLogRequest request)
            => await _progressLogService.AddProgressLogAsync(request).ToActionResultAsync(this);

        /// <summary>
        /// Deletes progress log.
        /// </summary>
        [HttpDelete(ProgressLog)]
        [ProducesResponseType(typeof(Result<Result<int>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProgressLogAsync([FromRoute] Int32 progressLogId)
            => await _progressLogService.DeleteProgressLogAsync(progressLogId).ToActionResultAsync(this);
    }
}
