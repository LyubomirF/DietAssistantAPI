using DietAssistant.DataAccess.Contracts;

#pragma warning disable

namespace DietAssistant.WebAPI.Extentions
{
    public static class WebApplicationExtensions
    {
        public static async Task InitializeApplication(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();

            await dbInitializer.Initialize();
        }
    }
}
