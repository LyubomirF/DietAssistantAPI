namespace DietAssistant.Business.Contracts.Models.Authentication
{
    public class AuthenticationResponse
    {
        public String AccessToken { get; set; }

        public Int32 ExpiresIn { get; set; }

        public String Type { get; set; }
    }
}
