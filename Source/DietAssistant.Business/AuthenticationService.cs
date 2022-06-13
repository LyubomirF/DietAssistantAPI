using DietAssistant.Business.Configuration;
using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Authentication;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DietAssistant.Business
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthConfiguration _config;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserRepository _userRepository;

        public AuthenticationService(
            IOptions<AuthConfiguration> options,
            IPasswordHasher<User> passwordHasher,
            IUserRepository userRepository)
        {
            _config = options.Value;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
        }

        public async Task<Result<AuthenticationResponse>> AuthenticateWithPasswordAsync(AuthenticationRequest request)
        {
            if (request is null)
                return Result
                    .CreateWithError<AuthenticationResponse>(EvaluationTypes.InvalidParameters, "Request model is requred.");

            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user is null)
                return Result
                    .CreateWithError<AuthenticationResponse>(EvaluationTypes.NotFound, "User with email was not found.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
                return Result
                    .CreateWithError<AuthenticationResponse>(EvaluationTypes.InvalidParameters, "Incorrect password.");

            var response = new AuthenticationResponse
            {
                AccessToken = GetAccessTokenAsync(user),
                Type = "Bearer",
                ExpiresIn = _config.TokenLifetimeSeconds
            };

            return response.AccessToken == null
                ? Result.CreateWithError<AuthenticationResponse>(EvaluationTypes.Unauthorized, "Unauthorized.")
                : Result.Create(response);
        }

        public async Task<Result<Boolean>> RegisterAsync(RegisterRequest request)
        {
            // TODO: Validate email
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user is not null)
                return Result
                    .CreateWithError<Boolean>(EvaluationTypes.InvalidParameters, "User with email already exists.");

            if (request.Password != request.ConfirmPassword)
                return Result
                    .CreateWithError<Boolean>(EvaluationTypes.InvalidParameters, "Passwords must match.");

            var newUser = new User
            {
                Email = request.Email,
                Name = request.Name,
            };

            var hashedPassword = _passwordHasher.HashPassword(newUser, request.Password);

            newUser.PasswordHash = hashedPassword;

            await _userRepository.SaveEntityAsync(newUser);

            return Result.Create(true);
        }

        private string GetAccessTokenAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var signingKey = new SymmetricSecurityKey(_config.SecurityKeyAsBytes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(GetClaims(user)),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config.Issuer,
                Audience = _config.Audience,
                Expires = DateTime.UtcNow.AddSeconds(_config.TokenLifetimeSeconds)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }

        private static IEnumerable<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            return claims;
        }
    }
}
