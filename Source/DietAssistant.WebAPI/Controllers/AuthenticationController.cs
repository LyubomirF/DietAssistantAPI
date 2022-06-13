using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Authentication;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
            => _authenticationService = authenticationService;

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> AuthenticateWithPasswordAsync([FromBody] AuthenticationRequest request)
            => await _authenticationService.AuthenticateWithPasswordAsync(request).ToActionResult(this);

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
            => await _authenticationService.RegisterAsync(request).ToActionResult(this);
    }
}
