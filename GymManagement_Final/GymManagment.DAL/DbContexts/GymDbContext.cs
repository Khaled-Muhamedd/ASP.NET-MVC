using GymManagment.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymManagment.DAL.DbContexts
{
    public class GymDbContext:IdentityDbContext<ApplicationUser>
    {
        public GymDbContext(DbContextOptions<GymDbContext> options):base(options) 
        {
            
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=.;Database=GymManagment;Trusted_Connection=true;TrustServerCertificate=true");
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<ApplicationUser>(eb =>
            {
                eb.Property(x => x.FirstName)
                .HasColumnType("varchar")
                .HasMaxLength(50);

                eb.Property(x => x.LastName)
                .HasColumnType("varchar")
                .HasMaxLength(50);
            });
        }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Booking> bookings { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<HealthRecord> healthRecords { get; set; }
        public DbSet<Member> members { get; set; }
        public DbSet<Membership> memberships { get; set; }
        public DbSet<Session> sessions { get; set; }
        public DbSet<Trainer> trainers { get; set; }

        //Not Nessaasary Because I Already Inherited From "IdentityDbContext"
        
        //public DbSet<ApplicationUser> Users { get; set; }
        //public DbSet<IdentityRole> Roles { get; set; }
        ////Relation Between User And Role Is Many To Many[User - Role] ==>IdentityUserRole
        //public DbSet<IdentityUserRole<string>> UserRoles { get; set; }



    }
}
