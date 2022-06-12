using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Authentication;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DietAssistant.WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
            => _authenticationService = authenticationService;

        [HttpPost]
        public async Task<IActionResult> AuthenticateWithPasswordAsync([FromBody] AuthenticationRequest request)
            => await _authenticationService.AuthenticateWithPasswordAsync(request).ToActionResult(this);


    }
}
