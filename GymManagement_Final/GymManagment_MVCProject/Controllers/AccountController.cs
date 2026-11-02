using GymManagment.BLL.ViewModels.LoginViewModels;
using GymManagment.DAL.Models;
using GymManagment_MVCProject.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<ApplicationUser> userManager ,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }
        //No Costomized Service

        //Get "Login" -> Show Form     
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        ////Post "Login" -> Submit Form
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) 
            {
                ModelState.AddModelError("Invalid Login" , "Invalid Email Or Password");
                return View(model);
            }
            var result = await signInManager.PasswordSignInAsync(user, model.Password , model.RememberMe , false);
            
            if(result.Succeeded)
            {
                logger.LogInformation($"User {user.UserName} Is Signed In");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (result.IsLockedOut)
            {
                logger.LogWarning($"User {user.UserName} Is Locked Out");
                ModelState.AddModelError("Invalid Login", "This Account Is Locked , Try Agin Later");
                return View(model);
            }
            else
            {
                ModelState.AddModelError("Invalid Login", "Invalid Email Or Password");
                return View(model);
            }


                
        }


        //Post "Logout" -> Button
        [HttpPost]
        [Authorize]
        public async Task< IActionResult> Logout() 
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }




        //Get "AccessDenied" View
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
