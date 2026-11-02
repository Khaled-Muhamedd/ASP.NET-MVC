using GymManagment.DAL.DataSeeding;
using GymManagment.DAL.DbContexts;
using GymManagment.DAL.Models;
using GymManagment_MVCProject;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagment.PL
{
    public static class ProgramExtensions
    {
        public static async Task MigrateAndSeedDatabaseAsync(this WebApplication app)
        {
            // before any middleware we must seeding the data before any request
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var roleManageer = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManageer = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var pendingMigration = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigration.Any())
            {
                logger.LogInformation($"Appling {pendingMigration.Count()} Pending Migrations ");
                await dbContext.Database.MigrateAsync();
            }
            var seedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");

            await GymDataSeeding.SeedAsync(dbContext, seedFolderPath, logger);

            await IdentityDataSeeding.SeedIdentityDataAsync(roleManageer , userManageer , logger); 
        }
    }
}
