using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Identity.Data;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.DataSeeding;

internal class IdentityDataSeeder: IDataSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly StoreIdentityDbContext _dbContext;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<IdentityDataSeeder> _logger;

    public IdentityDataSeeder(UserManager<ApplicationUser> userManager, StoreIdentityDbContext dbContext, RoleManager<IdentityRole> roleManager, ILogger<IdentityDataSeeder> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _roleManager = roleManager;
        _logger = logger;
    }
    public async Task SeedAsync(CancellationToken ct = default)
    {
        try
        {
            var pendingMigration = await _dbContext.Database.GetPendingMigrationsAsync(ct);
            if (pendingMigration.Any())
                await _dbContext.Database.MigrateAsync(ct);

            if (!await _roleManager.Roles.AnyAsync(ct))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            }

            if (!await _userManager.Users.AnyAsync(ct))
            {
                var admin = new ApplicationUser
                {
                    DisplayName = "AbdelrahmanKeshk",
                    Email = "Abdo@gmail.com",
                    UserName = "AbdelrahmanKeshk",
                    PhoneNumber = "01000318871",

                };
                var createResult = await _userManager.CreateAsync(admin, "P@ssw0rd!");

                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "SuperAdmin");
                }
                else
                {
                    var Erorrs = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogWarning($"Failed to create admin : {Erorrs}");
                }
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding identity data.");
            return;
        }
    }
}
