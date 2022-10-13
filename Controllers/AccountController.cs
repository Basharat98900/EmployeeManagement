using System.IO;
using System.Threading.Tasks;
using EF_DotNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EF_DotNetCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicaitonUsers> identityUser;
        private readonly SignInManager<ApplicaitonUsers> signInManager;
        public AccountController(SignInManager<ApplicaitonUsers> signInManager,UserManager<ApplicaitonUsers> identityUser)
        {
            this.signInManager = signInManager;
            this.identityUser = identityUser;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUser obj)
        {
            
            if(ModelState.IsValid)
            {
                //await  IsEmailInUse(obj.UserName);
                var user = new ApplicaitonUsers
                {
                    UserName = obj.UserName,
                    Email = obj.UserName,
                    gender = obj.Gender,
                
                    

                };
            var result = await identityUser.CreateAsync(user, obj.Password);

                if (result.Succeeded)
                {
                    if(signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListOfUsers", "Admin");
                    }
                    await signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Contact", "Home");
                }
                foreach(var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Contact","Home");
        }

        
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginView loginView,string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(loginView.UserName, loginView.Password,loginView.RememberMe,false);
                if(result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return LocalRedirect(returnUrl);
                        }

                    }
                    return RedirectToAction("Contact", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "UserName or Password is incorrect");
                }
                
            }
            return View(loginView);
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> EmailInUse(string UserName)
        {
            var user = await identityUser.FindByEmailAsync(UserName);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"User with {user.Email} already exists");
            }
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
