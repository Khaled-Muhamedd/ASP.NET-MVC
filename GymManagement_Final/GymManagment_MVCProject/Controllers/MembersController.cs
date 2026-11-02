using GymManagment.BLL.Services.Attachement;
using GymManagment.BLL.Services.Classes;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.MemberViewModels;
using GymManagment.DAL.Models;
using GymManagment.DAL.Repositories.Classes;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagment.PL.Controllers
{
    [Authorize (Roles ="SuperAdmin")]
    public class MembersController : Controller
    {
        private readonly IMemberServices _memberServices;
        private readonly IAttachmentService attachmentService;

        public MembersController(IMemberServices memberServices, IAttachmentService attachmentService)
        {
            this._memberServices = memberServices;
            this.attachmentService = attachmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Picture(int id)
        {
            var member = await _memberServices.GetMemberDetailsByTdAsync(id);
            if (member is null || string.IsNullOrEmpty(member.Photo))
                return NotFound();


            var result = attachmentService.GetFile(member.Photo, "MembersPictures");
            if (result is null) return NotFound();

            return File(result.Value.Stream, result.Value.ContentType);
        }


        //GET BaseUrl/Members/Index
        //Index -list all members
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberServices.GetAllMembersAsync();
            return View(members);
        }

        //GET BaseUrl/Members/MembersDetails/{id}
        //MembersDetails -show all member's details

        [HttpGet]
        public async Task<IActionResult> MembersDetails(int id, CancellationToken ct) 
        {
            #region Logic
            //In Service
            //Get Member Id
            //Ckeck->IsMember Equal null? (if it  null--> return Index With Message)
            // else member is not null (retutn view with data )
            #endregion
            var member = await _memberServices.GetMemberDetailsByTdAsync(id, ct);
            if (member == null) 
            {
                TempData["ErrorMessage"] = "Member not found";
                return RedirectToAction(nameof(Index));

            }
           
            return View("MembersDetails",member);
        }


        //GET BaseUrl/Members/HealthRecordDetails/{id}
        //HealthRecordDetails -show all member's HealthRecordDetails
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            #region logic
            //Get HealthRecord By Member Id
            //Ckeck->IsHealthRecord Equal null? (if it  null--> return Index With Message)
            // else HealthRecord is not null (retutn view with data )
            #endregion
            var HealthRecord=await _memberServices.GetHealthRecordDetailsByTdAsync(id,ct);
            if (HealthRecord == null) {
                TempData["ErrorMessage"] = "HealthRecord not found";
                return RedirectToAction(nameof(Index));
            }
            return View(HealthRecord);
        }

        #region Create Member
        //GET BaseUrl/Members/Create
        //Create -show Empty form

        [HttpGet]
        public IActionResult Create() => View();

        //POST BaseUrl/Members/Create {Member}
        //Create -save submittes form

        #region ViewData & ViewBag & TempData
        //properties inhertied from "Controller" in MVC only to transfer data from place to another(action->view | view->partial view | view->layout | action->action)
        //ViewData & ViewBag-->A2der A3mel transfer the data within the same Request only "و بعد م الطلب يخلص الداتا بتطير"
        //but "TempData" --> A2der A3mel transfer the data we el data tkmel m3aya to the previous Request (usecase-->"Redirection:transfer data from request to another request"-->due to its lifetime)

        #endregion

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);
            var result = await _memberServices.CreateMemberAsync(model, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Creat Member";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit Member
        //GET BaseUrl/Members/Edit/{id}
        //Edit -show form per-filled
        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var result = await _memberServices.GetMemberToUpdate(id, ct);

            if (!result.success || result.value == null)
            {
                TempData["ErrorMessage"] = "Member Is Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(result.value); // ⬅️ كده بقى بيبعت MemberToUpdateViewModel الصح
        }

        //POST BaseUrl/Members/Edit/{Member}
        //Edit -save edits
        [HttpPost]
        public async Task<IActionResult> EditMember(int id ,MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _memberServices.UpdateMemberDetailsAsync(id,model, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Member Update Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Update Member";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete Member
        //Get BaseUrl/Members/Delete/{id}
        //Delete -Show form confirmation

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct) 
        {
            var member=await _memberServices.GetMemberDetailsByTdAsync(id,ct);
            if(member==null)
            {
                TempData["ErrorMessage"] = "Member not found";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        //POST BaseUrl/Members/DeleteConfirmed/{id}
        //DeleteConfirmed -Delete after confirmation
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute]int id,CancellationToken ct)
        {
            var result = await _memberServices.RemoveMemberAsync(id,ct);
            if (result.success)
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Delete Member";
            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}