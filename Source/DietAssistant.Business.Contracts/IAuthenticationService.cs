using DietAssistant.Business.Contracts.Models.Authentication;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IAuthenticationService
    {
        Task<Result<AuthenticationResponse>> AuthenticateWithPasswordAsync(AuthenticationRequest request);
    }
}
