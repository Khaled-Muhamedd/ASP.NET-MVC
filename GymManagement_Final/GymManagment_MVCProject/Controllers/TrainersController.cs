using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class TrainersController : Controller
    {
        private readonly ITrainerServices trainerServices;

        public TrainersController(ITrainerServices trainerServices)
        {
            this.trainerServices = trainerServices;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var trainers = await trainerServices.GetAllTrainersAsync();
            return View(trainers);
        }
        

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid) return View(model);
            var trainer = await trainerServices.CreateTrainerAsync(model,ct);
            if (trainer.success)
            {
                TempData["SuccessMessage"] = "Trainer Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = trainer.error;
            return View(model);
        }

        [HttpGet]

        public async Task<IActionResult> Details(int id , CancellationToken token)
        {
            var trainer= await trainerServices.GetTrainerDetailsAsync(id, token);
            if(trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct)
        {
            var trainer = await trainerServices.GetTrainerToUpdateAsync(id, ct);
            if(trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateTrainerViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var trainer = await trainerServices.UpdateTrainerAsync(id, model, ct);
            if (trainer.success)
            {
                TempData["SuccessMessage"] = "Trainer updated Successfully";
            }
            else
                TempData["ErrorMessage"] = trainer.error;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken token)
        {
            var trainer = await trainerServices.GetTrainerDetailsAsync(id, token);
            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer); // يعرض confirmation page
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id , CancellationToken token)
        {
            var trainer = await trainerServices.RemoveTrainerAsync(id, token);
            if (trainer.success)
            {
                TempData["SuccessMessage"] = "Trainer deleted Successfully";
            }
            else
                TempData["ErrorMessage"] = trainer.error;
            return RedirectToAction(nameof(Index));
        }

    }
}
