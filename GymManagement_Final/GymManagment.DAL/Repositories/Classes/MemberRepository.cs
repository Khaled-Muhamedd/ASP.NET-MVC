using GymManagment.DAL.DbContexts;
using GymManagment.DAL.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Repositories.Classes
{
    public class MemberRepository :GenericRepository<Member>, IMemberRepository
    {
        private GymDbContext dbContext;
      
        public MemberRepository(GymDbContext dbContext):base(dbContext)
        {
            this.dbContext = dbContext;
        }
       //Implement New Feature from Spacific Interface

    }
}
