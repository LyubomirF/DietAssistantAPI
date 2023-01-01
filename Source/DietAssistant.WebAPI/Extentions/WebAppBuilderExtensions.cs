#pragma warning disable

namespace DietAssistant.WebAPI.Extentions
{
    public static class WebAppBuilderExtensions
    {
        public static void AddAppConfiguration(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("AppConfig");

            if (!string.IsNullOrEmpty(connectionString))
            {
                builder.Configuration.AddAzureAppConfiguration(connectionString);
            }
        }
    }
}
