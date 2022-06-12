using System.ComponentModel.DataAnnotations;

namespace DietAssistant.Business.Contracts.Models.Authentication
{
    public class AuthenticationRequest
    {
        [Required]
        public String Email { get; set; }

        [Required]
        public String Password { get; set; }
    }
}
