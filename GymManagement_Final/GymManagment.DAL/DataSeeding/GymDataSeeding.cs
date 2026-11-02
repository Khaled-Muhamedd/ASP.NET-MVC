using GymManagment.DAL.DbContexts;
using GymManagment.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymManagment.DAL.DataSeeding
{
    public class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext dbContext , string seedFolderPath, ILogger logger, CancellationToken ct =default)
        {
            //ILogger---> framework have loogger to log "audit" inside console
            //seedFolderPath--->Location
            try
            {
				if(!await dbContext.Plans.AnyAsync(ct))
				{
                    var plans = LoadDataFromJsonFile<Plan>(seedFolderPath, "plans.json");
                    if(plans.Any())
                    {
                        dbContext.Plans.AddRange(plans);
                        logger.LogInformation($"Plans Seeded With Count ={plans.Count}");
                    }
                    if (dbContext.ChangeTracker.HasChanges())
                        await dbContext.SaveChangesAsync();
                    else
                        logger.LogInformation("Plan Already Seeded");
                }
			}
			catch (Exception ex)
			{
                logger.LogError(ex, "Gym Data Seeding Failed");
                throw;
			}
        }
        private static List<T> LoadDataFromJsonFile<T>(string folderPath,string fileName) 
        {
            var filePath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed Data File Not Found : {filePath}");

            var data = File.ReadAllText(filePath); // read data inside "filePath"
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
        }
    }
}