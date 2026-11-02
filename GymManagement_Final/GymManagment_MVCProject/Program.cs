using GymManagment.BLL;
using GymManagment.BLL.Services.Attachement;
using GymManagment.BLL.Services.Attachment;
using GymManagment.BLL.Services.Classes;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.DAL.DataSeeding;
using GymManagment.DAL.DbContexts;
using GymManagment.DAL.Models;
using GymManagment.DAL.Repositories.Classes;
using GymManagment.DAL.Repositories.Interfaces;
using GymManagment.PL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagment_MVCProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Da container el debendancy injection ---> hwa elly b create concrete instance "new object" and then send it inside constractor
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<GymDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaulteConnection"));

            });
          
            //builder.Services.AddScoped<IPlanRepository, PlanRepository>();//lma 7d mn el constractors ytlop mnk 7aga mn no3 IPlanRepository a3mel inject l object mn no3 PlanRepository
            //builder.Services.AddScoped<IMemberRepository, MemberRepository>();
            #region 
            //determine lifetime "dealocation - despose"
            //builder.Services.AddScoped ----->per request lifetime of object is "within the request then dealocated it" ya3ny 3ady atlbo 20 mara fe nafs el request
            //builder.Services.AddSingleton--->per lifetime of whole application "stateless utilities"
            //builder.Services.AddTransient--->per operation "if one request have three operation the object id dealocated in every end of each operation and will created new one to previous operation"
            #endregion
            builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ISessionReposaitory, SessionReposaitory>();
            builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();



            builder.Services.AddScoped<IMemberServices, MemberServices>();
            builder.Services.AddScoped<IPlanServices, PlanServices>();
            builder.Services.AddScoped<ITrainerServices, TrainerServices>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<IAnalyticsServices, AnalyticsServices>();
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IMembershipService, MembershipService>();


            builder.Services.AddAutoMapper(m => m.AddProfile(new MappingProfile()));
            builder.Services.AddScoped<GymDbContext>();// not  nessacary
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                //config.Password.RequireLowercase=true;//Default
                //config.Password.RequireUppercase=true;//Default
                //config.Password.RequiredLength=6;     //Default

                config.User.RequireUniqueEmail = true;
                config.Lockout.DefaultLockoutTimeSpan= TimeSpan.FromMinutes(2);
                config.Lockout.MaxFailedAccessAttempts = 5;

            })
                .AddEntityFrameworkStores<GymDbContext>();


            builder.Services.ConfigureApplicationCookie(options =>
            {
                // redirect unauthenticated users (401)
                options.LoginPath = "/Account/Login";
                // redirect forbidden users (403)
                options.AccessDeniedPath = "/Account/AccessDenied";
            });// Default Paths


            var app = builder.Build();
            // before any middleware we must seeding the data before any request
            await app.MigrateAndSeedDatabaseAsync();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
