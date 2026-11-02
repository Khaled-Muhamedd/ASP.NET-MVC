using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.PlanViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment_MVCProject.Controllers
{
    [Authorize]
    public class PlanController : Controller
    {
        //composition
        //private readonly GymDbContext dbContext = new GymDbContext(); //wrong 
        //private readonly IPlanRepository planRepository =new PlanRepository(); // also wrong but Enhanced
        private readonly IPlanServices planServices;

        public PlanController(IPlanServices planServices)// dependancy injection
        {
            this.planServices = planServices;
        }
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken token)//CancellationToken ct ---> came from browser "by default"
        {
            var plans = await planServices.GetAllPlansAsync(); // Fale --> because ith's not tracked
            return View(plans);// View with Model
        }
       
        [HttpGet]
        public async Task<IActionResult> Details(int id , CancellationToken token)
        {
            var plan = await planServices.GetPlanByIdAsync(id, token);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var plan = await planServices.GetPlanToUpdateAsync(id, ct);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan cannot be edited (not found, inactive, or has active memberships).";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id , UpdatePlanViewModel model, CancellationToken token)
        {
            if(!ModelState.IsValid) return View(model);
            var plan = await planServices.UpdatePlanAsync (id, model , token);
            if (plan.success)
            {
                TempData["SuccessMessage"] = "Plan Updated Successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = plan.error;
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Activate(int id)
        {
            var plan = await planServices.ToggleActivationAsync (id);
            if (plan.success)
            {
                TempData["SuccessMessage"] = "Plan State Changes";
            }
            else
            {
                TempData["ErrorMessage"] = plan.error;
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
