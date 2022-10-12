using EF_DotNetCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Localization.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EF_DotNetCore.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public readonly UserManager<ApplicaitonUsers> usermanager;

        public AdminController(RoleManager<IdentityRole> roleManager,UserManager<ApplicaitonUsers> userManager )
        {
            this.roleManager = roleManager;
            usermanager = userManager;
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel obj)
        {
            if (ModelState.IsValid)
            {
                var identityRole = new IdentityRole()
                {
                    Name = obj.RoleName
                };
                var role = await roleManager.CreateAsync(identityRole);
                if (role.Succeeded)
                {
                    return RedirectToAction("GetRoles","Admin");
                }
                else
                {
                    foreach(var a in role.Errors)
                    {
                        ModelState.AddModelError(string.Empty, a.Description);
                    }
                    
                }
            }
            return View(obj);
        }

        [HttpGet]
        public async Task<IActionResult> RoleEditView(string ID)
        {
            var role = await roleManager.FindByIdAsync(ID);
            var model = new RoleEditModel();
            if (role != null)
            {
                model.Id = role.Id;
                model.RoleName = role.Name;
                foreach (var users in usermanager.Users.ToList())
                {
                    if (await usermanager.IsInRoleAsync(users, role.Name))
                    {
                        model.Users.Add(users.UserName);
                    }
                }
                return View(model);

            }
            else
            {
                ViewBag.ErrorMsg = $"Role witth ID: {ID} not found";
                return View("ViewNotFound");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RoleEditView(RoleEditModel model)
        {
            var role=await roleManager.FindByIdAsync(model.Id);
            if(role != null)
            {
                role.Name = model.RoleName;
                var result=await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("GetRoles", "Admin");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }
                }
            }
            else
            {
                ViewBag.ErrorMsg = $"Role witth ID: {model.Id} not found";
                return View("ViewNotFound");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string ID)
        {
            ViewBag.RoleId = ID;    
            var role=await roleManager.FindByIdAsync(ID);
            if(role==null)
            {
                ViewBag.ErrorMsg = $"The Role wiht ID:{ID} not found";
                return View("ViewNotFound");
            }
            else
            {
                var model =new List<UserRoleViewModel>();

                foreach (var item in usermanager.Users.ToList())
                {
                    var a = new UserRoleViewModel();
                    a.UserName = item.UserName;
                    a.UserID = item.Id;
                    if(await usermanager.IsInRoleAsync(item, role.Name))
                    {
                        a.IsSelected = true;
                    }
                    else
                    {
                        a.IsSelected = false;
                    }
                    model.Add(a);
                }
                return View(model);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> list)
        //{

        //}
    }
}
