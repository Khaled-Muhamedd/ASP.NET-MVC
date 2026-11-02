using GymManagment.DAL.DbContexts;
using GymManagment.DAL.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Classes
{
    public class PlanRepository :GenericRepository<Plan>, IPlanRepository
    {
        //private readonly GymDbContext dbContext = new GymDbContext(); // still wrong
        private GymDbContext dbContext;
        public PlanRepository(GymDbContext dbContext):base(dbContext)
        {
           this.dbContext= dbContext;
        }
        //Implement New Feature from Spacific Interface
    }
}
