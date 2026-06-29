using E_Commerce.Domain.Contracts;

namespace E_Commerce.API.Extentions
{
    public static class WebAppExtentions
    {
        public static async Task<WebApplication> SeedAndMigrateDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dataSeeder = scope.ServiceProvider.GetRequiredKeyedService<IDataSeeder>("Catalog");

            await dataSeeder.SeedAsync();
            return app;
        }
    }
}
