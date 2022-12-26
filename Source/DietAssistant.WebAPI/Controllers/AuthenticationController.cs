using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Authentication;
using DietAssistant.Common;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable

namespace DietAssistant.WebAPI.Controllers
{
    using static AuthRoutes;

    [Route(Auth)]
    [Authorize]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
            => _authenticationService = authenticationService;

        /// <summary>
        /// Password authentication.
        /// </summary>
        [HttpPost(Login)]
        [ProducesResponseType(typeof(Result<AuthenticationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        public async Task<IActionResult> AuthenticateWithPasswordAsync([FromBody] AuthenticationRequest request)
            => await _authenticationService.AuthenticateWithPasswordAsync(request).ToActionResultAsync(this);


        /// <summary>
        /// Register with email and password.
        /// </summary>
        [HttpPost(Register)]
        [ProducesResponseType(typeof(Result<Boolean>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
            => await _authenticationService.RegisterAsync(request).ToActionResultAsync(this);
    }
}
