using GymManagment.BLL.Services.Interfaces;
using GymManagment_MVCProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GymManagment_MVCProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IAnalyticsServices analyticsServices;

        public HomeController( ILogger<HomeController> logger , IAnalyticsServices analyticsServices )
        {
            this.logger = logger;
            this.analyticsServices = analyticsServices;
        }
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var data = await analyticsServices.GetDataAsync(token);
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
