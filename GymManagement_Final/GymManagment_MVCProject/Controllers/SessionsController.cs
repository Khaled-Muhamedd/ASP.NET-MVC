using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Classes;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace GymManagment.PL.wwwroot
{
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly ISessionService sessionService;

        public SessionsController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions= await sessionService.GetAllSessionsAsync(ct);
            return View(sessions);
        }

        [HttpGet]
        public async Task< IActionResult >Create() 
        {
            await PopulateDropdownsAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(nameof(Create), model);
            }
            var result = await sessionService.CreateSessionAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                await PopulateDropdownsAsync();
                return View(nameof(Create), model);
            }
        }

        private async Task PopulateDropdownsAsync()
        {
            ViewBag.Trainers = new SelectList(await sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await sessionService.GetCategoriesForDropDownAsync(), "Id", "CategoryName");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var session=await sessionService.GetSessionByIdAsync(id,ct);//
            if (session.success)
                return View(session.value);
            else
            {
                TempData["ErrorMessage"] = session.error;
                return RedirectToAction(nameof(Index));
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken token)
        {
            var result =await sessionService.GetSessionToUpdateAsync(id, token );
            if (result.success)
            {
                ViewBag.Trainers = new SelectList(await sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(result.value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,UpdateSessionViewModel model ,CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Trainers = new SelectList(await sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(model);
            }
            var result=await sessionService.UpdateSessionAsync(id,model, token);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Updated";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"]=result.error;
                ViewBag.Trainers = new SelectList(await sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(model);
            }

        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id , CancellationToken token)
        {
            var result = await sessionService.GetSessionByIdAsync(id);
            if (result.success) 
            {
                return View(result.value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id , CancellationToken token)
        {
            var result= await sessionService.RemoveSessionAsync(id , token);
            //if (result.success) 
            //{
            //    TempData["SuccessMessage"] = "Session Removed";
            //    return RedirectToAction(nameof(Index));
            //}
            //else
            //{
            //    TempData["ErrorMessage"] = result.error;
            //    return RedirectToAction(nameof(Index));
            //}
            // or-- >
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Session Deleted " : result.error;
            return RedirectToAction(nameof(Index));
        }

    }
}
