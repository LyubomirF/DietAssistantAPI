using DietAssistant.DataAccess.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DietAssistant.DataAccess
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly DietAssistantDbContext _dietAssistantDbContext;

        public DatabaseInitializer(DietAssistantDbContext dietAssistantDbContext)
            => _dietAssistantDbContext = dietAssistantDbContext;

        public async Task Initialize()
        {
            await ApplyMigrations();
        }

        private async Task ApplyMigrations()
        {
            var pendingMigrations = await _dietAssistantDbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                await _dietAssistantDbContext.Database.MigrateAsync();
            }
        }
    }
}
