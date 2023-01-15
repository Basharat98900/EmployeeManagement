using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using EF_DotNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace EF_DotNetCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicaitonUsers> userManager;
        private readonly SignInManager<ApplicaitonUsers> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(SignInManager<ApplicaitonUsers> signInManager, UserManager<ApplicaitonUsers> identityUser, ILogger<AccountController> logger)
        {
            this.signInManager = signInManager;
            this.userManager = identityUser;
            this.logger = logger;
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

            if (ModelState.IsValid)
            {
                //await  IsEmailInUse(obj.UserName);
                var user = new ApplicaitonUsers
                {
                    UserName = obj.UserName,
                    Email = obj.UserName,
                    gender = obj.Gender,



                };
                var result = await userManager.CreateAsync(user, obj.Password);

                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confrimationLink = Url.Action("ConfirmEmail", "Account", new {userId=user.Id,token=token},Request.Scheme);
                    logger.LogInformation(confrimationLink);

                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListOfUsers", "Admin");
                    }

                   // bool isMailSend = SendEmail(obj.UserName, confrimationLink);
                    //if(isMailSend)
                    //{
                        ViewBag.ErrorTitle = "Registeration Successful";
                        ViewBag.ErrorMessage = "Please confirm the email before loggin in";
                    //}
                    //else
                    //{
                    //    ViewBag.ErrorTitle = "Mail not send";
                    //    ViewBag.ErrorMessage = "Please contact the admin";
                    //}
                    

                    return View("ViewNotFound");
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }
            return View(obj);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token == null)
            {
                return View("Index","Home");
            }
            var user=await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorTitle = $"The User ID {userId} is invalid";
                return View("ViewNotFound");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            if(result.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("ViewNotFound");

        }

        public bool SendEmail(string userEmail,string confirmationLink)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From=new MailAddress("basharatk189@gmail.com");
            mailMessage.To.Add(new MailAddress("basharatk189@gmail.com"));
            mailMessage.Subject = "Confirm you Email";
            mailMessage.IsBodyHtml = false;
            mailMessage.Body = confirmationLink;
            
            SmtpClient client = new SmtpClient();
            
            client.UseDefaultCredentials = true;
            client.Credentials = new System.Net.NetworkCredential("basharatk189@gmail.com", "98900@Khan");
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Port = 587;

            try
            {
                client.Send(mailMessage);
                return true;
            } 
            catch (SmtpException ex)
            {
                logger.LogInformation(ex.Message+"\n");
                logger.LogInformation(ex.StackTrace+"\n");
                logger.LogInformation(ex.InnerException+"\n");
            }
            return false;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Contact", "Home");
        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginView loginView, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(loginView.UserName, loginView.Password, loginView.RememberMe,true);
                if (result.Succeeded)
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
                if (result.IsLockedOut)
                {
                    return View("LockOutView");
                }

            }
            return View(loginView);
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> EmailInUse(string UserName)
        {
            var user = await userManager.FindByEmailAsync(UserName);
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login_ExternalProviders(string returnUrl)
        {
            Login_Google obj = new Login_Google()
            {
                returnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(obj);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login_ExternalProviders(Login_Google obj,string returnUrl)
        {
            obj.ExternalLogins=(await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user=await userManager.FindByEmailAsync(obj.email);
                if (user !=null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, obj.Password)))
                {
                    ModelState.AddModelError("","Email not confirmed yet check your email");
                    //var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var confrimationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                    //bool ismailSend = SendEmail(user.Email, confrimationLink);
                    //logger.LogInformation("\n"+ismailSend);
                    return View(obj);
                }

                var result = await signInManager.PasswordSignInAsync(user,obj.Password,obj.RememberMe,false);
                if (result.Succeeded)
                {
                    if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index","Home");
                    }
                }
                ModelState.AddModelError("", "Email or Password is incorrect");
            }
            return View(obj);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPassword obj)
        {
            if(ModelState.IsValid)
            {
                var user =await userManager.FindByEmailAsync(obj.Email);
                if(user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var redirectUrl=Url.Action("ResetPassword","Account",new { email=user.Email,token=token },Request.Scheme);
                    logger.LogInformation(redirectUrl);
                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(obj);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
            {
                ModelState.AddModelError("", "Inavalid Password token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel obj)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(obj.Email);
                if(user!=null)
                {
                    var result = await userManager.ResetPasswordAsync(user, obj.token, obj.ConfirmPassword);
                    if(result.Succeeded)
                    {
                        if(await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user,System.DateTimeOffset.UtcNow);
                        }
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View(obj);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(obj);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogins(string provider, string returnUrl)
        {
            var redirectUrl=Url.Action("ExternalLoginCallback","Account",new { ReturnUrl = returnUrl });
            var properties=signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);   
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            var userHasPassword=await userManager.HasPasswordAsync(user);
            if(!userHasPassword)
            {
                return View("AddPassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPassword obj)
        {

            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                var result= await userManager.AddPasswordAsync(user,obj.ConfirmPassword);
                if (result.Succeeded)
                {
                    await signInManager.RefreshSignInAsync(user);
                    ViewBag.Msg = "Added";
                    return View("ChangePasswordConfirmation");
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel obj)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if(user==null)
                {
                    return View("Login");
                }
                var result=await userManager.ChangePasswordAsync(user,obj.CurrentPassword,obj.ConfirmPassword);
                if (result.Succeeded)
                {
                    await signInManager.RefreshSignInAsync(user);
                    return View("ChangePasswordConfirmation");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(obj);
        }

        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await userManager.GetUserAsync(User);
            var userHasPassword = await userManager.HasPasswordAsync(user);
            if(userHasPassword)
            {
                return View("ChangePassword");
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl=null,string remoteError = null)
        {
            returnUrl=returnUrl ?? Url.Content("~/");
            var loginModel = new Login_Google()
            {
                returnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
               
            };
            
            if(remoteError != null)
            {
                ModelState.AddModelError(string.Empty,$"Error from external provider {remoteError}");
                return View("Login_ExternalProviders", loginModel);
            }

            var info=await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, $"Error loding the external login information");
                return View("Login_ExternalProviders", loginModel);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicaitonUsers user = null;
            if(email != null)
            {
                user =await userManager.FindByEmailAsync(email);

                if(user !=null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("", "Email not confirmed");
                    return View("Login_ExternalProviders",loginModel);
                }
            }


            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                if(email != null)
                {
                    if(user == null)
                    {
                        user = new ApplicaitonUsers()
                        {
                            UserName=info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email=info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                        await userManager.CreateAsync(user);
                    }
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                ViewBag.ErrorTitle = $"Email Claim not recieved from {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact to developer";
                return View("ViewNotFound");
            }

        }
    }
}
