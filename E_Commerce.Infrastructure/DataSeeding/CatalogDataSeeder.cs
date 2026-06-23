using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.DataSeeding
{
    internal class CatalogDataSeeder(StoreDbContext dbContext, ILogger<CatalogDataSeeder> logger) : IDataSeeder
    {
        public async Task SeedAsync(CancellationToken ct = default)
        {
            try
            {
                var PendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(ct);
                if (PendingMigrations.Any())
                    await dbContext.Database.MigrateAsync(ct);

                // "D:\Full Stack Diploma\Backend\09 API\E Commerce APP\E_Commerce\E_Commerce.API\bin\Debug\net8.0\DataSeed\products.json"
                var seedDataPath = Path.Combine(AppContext.BaseDirectory, "DataSeed");

                var productsFilePath = Path.Combine(seedDataPath, "products.json");
            }
            catch { }
        }

        private async Task SeedIfEmptyAsync<T, Tkey>(string rootPath, string FileName, CancellationToken ct) where T : BaseEntity<Tkey>
        {
            if(await dbContext.Set<T>().AnyAsync(ct)){
                logger.LogInformation("Table is not empty, skipping seeding");
                return;
            }

            var filePath = Path.Combine(rootPath, FileName);

            if (!File.Exists(filePath)) { 
                logger.LogWarning($"File {FileName} not found, skipping seeding");
                return;
            }
            using var fileStream = File.OpenRead(filePath);

           var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var items = await JsonSerializer.DeserializeAsync<List<T>>(fileStream, options, ct);

            if (items?.Any() ?? false) // Check if items is not null and has any elements  if (items.Count > 0) "Any" is preferred 
            {
                dbContext.Set<T>().AddRange(items);
            }
        }
    }
}
