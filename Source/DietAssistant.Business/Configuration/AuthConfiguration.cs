using System.Text;

#pragma warning disable

namespace DietAssistant.Business.Configuration
{
    public class AuthConfiguration
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SecurityKey { get; set; }

        public byte[] SecurityKeyAsBytes => Encoding.UTF8.GetBytes(SecurityKey);

        public int TokenLifetimeSeconds { get; set; }
    }
}
