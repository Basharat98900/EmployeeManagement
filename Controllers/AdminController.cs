using EF_DotNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EF_DotNetCore.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public readonly UserManager<ApplicaitonUsers> usermanager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicaitonUsers> userManager)
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
        [Authorize(Policy ="CreateRolePolicy")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "CreateRolePolicy")]
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
                    return RedirectToAction("GetRoles", "Admin");
                }
                else
                {
                    foreach (var a in role.Errors)
                    {
                        ModelState.AddModelError(string.Empty, a.Description);
                    }

                }
            }
            return View(obj);
        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
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
        [Authorize(Policy ="EditRolePolicy")]

        public async Task<IActionResult> RoleEditView(RoleEditModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role != null)
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

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
            var role = await roleManager.FindByIdAsync(ID);
            if (role == null)
            {
                ViewBag.ErrorMsg = $"The Role wiht ID:{ID} not found";
                return View("ViewNotFound");
            }
            else
            {
                var model = new List<UserRoleViewModel>();

                foreach (var item in usermanager.Users.ToList())
                {
                    var a = new UserRoleViewModel();
                    a.UserName = item.UserName;
                    a.UserID = item.Id;
                    if (await usermanager.IsInRoleAsync(item, role.Name))
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> list, string ID)
        {
            var role = await roleManager.FindByIdAsync(ID);
            if (role == null)
            {
                ViewBag.ErrorMsg = $"Role with ID:{ID} does not exists";
                return View("ViewNotFound");
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var user = await usermanager.FindByIdAsync(list[i].UserID);
                    IdentityResult result = null;
                    if (list[i].IsSelected && !(await usermanager.IsInRoleAsync(user, role.Name)))
                    {
                        result = await usermanager.AddToRoleAsync(user, role.Name);
                    }
                    else if (!list[i].IsSelected && (await usermanager.IsInRoleAsync(user, role.Name)))
                    {
                        result = await usermanager.RemoveFromRoleAsync(user, role.Name);
                    }
                    else
                    {
                        continue;
                    }
                    if (result.Succeeded)
                    {
                        if (i < list.Count - 1)
                        {
                            continue;
                        }
                        else
                        {
                            return RedirectToAction("RoleEditView", new { id = ID });
                        }
                    }
                }
                return RedirectToAction("RoleEditView", new { id = ID });
            }
        }

        [HttpGet]
        public IActionResult ListOfUsers()
        {

            var userModel = usermanager.Users;
            return View(userModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await usermanager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMsg = $"User with ID: {id} not found";
                return View("ViewNotFound");
            }
            else
            {
                var claim = await usermanager.GetClaimsAsync(user);
                var role = await usermanager.GetRolesAsync(user);
                EditUserViewModel a = new EditUserViewModel()
                {
                    ID = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Claims = claim.Select(c =>c.Type+": "+ c.Value).ToList(),
                    Roles = role,
                    gender = user.gender
                };

                return View(a);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel obj)
        {
            var user = await usermanager.FindByIdAsync(obj.ID);
            if (user == null)
            {
                ViewBag.ErrorMsg = $"User with ID: {obj.ID} cannot be found";
                return View("ViewNotFound");
            }
            else
            {
                user.UserName = obj.UserName;
                user.Email = obj.Email;
                user.gender = obj.gender;

                var result = await usermanager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListOfUsers", "Admin");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user =await usermanager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMsg = $"User with ID: {id} cannot be found";
                return View("ViewNotFound");
            }
            else
            {
                try
                {
                    var result = await usermanager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListOfUsers", "Admin");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
                catch(DbUpdateException ex)
                {
                    ViewBag.ErrorTitle = $"{user.UserName} has a role";
                    ViewBag.ErrorMessage = "Cannot delete a User who has a role or Claim";
                    
                    return View("ViewNotFound");
                }
            }
            return View("ListOfUsers");
        }


        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMsg = $"Role with ID: {id} cannot be found";
                return View("ViewNotFound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("GetRoles", "Admin");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                    return View("GetRoles");
                }
                catch (DbUpdateException ex)
                {
                    ViewBag.ErrorTitle = $"{role.Name} is in Use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete";
                    return View("ViewNotFound");
                }
            }
            
        }
        [HttpGet("EditUserRoles/id")]
        [Authorize(Policy = "RolePolicy")]
        public async Task<IActionResult> EditUserRoles(string id)
        {
            ViewBag.UserID = id;
            var user =await usermanager.FindByIdAsync(id);
            if(user == null)
            {
                ViewBag.ErrorMsg = $"User with UserID: {id} cannot be found";
                return View("ViewNotFound");
            }
            else
            {
                var model = new List<UserRolesModel>();
                foreach (var item in roleManager.Roles.ToList())
                {
                    var userrolemodel = new UserRolesModel()
                    {
                        RoleName = item.Name,
                        RoleID = item.Id
                    };
                    if (await usermanager.IsInRoleAsync(user, userrolemodel.RoleName))
                    {
                        userrolemodel.IsSelected = true;
                    }
                    else
                    {
                        userrolemodel.IsSelected = false;
                    }
                    model.Add(userrolemodel);
                }
                return View(model);
            }
        }

        [HttpPost("EditUserRoles/id")]
        [Authorize(Policy = "RolePolicy")]


        public async Task<IActionResult> EditUserRoles(List<UserRolesModel> list,string id)
        {
            var user = await usermanager.FindByIdAsync(id); 
            if(user==null)
            {
                ViewBag.ErrorMsg = $"User with the UserID:{id} not found";
                return View("ViewNotFound");
            }
            var roles=await usermanager.GetRolesAsync(user);
            var result =await usermanager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "cannot reomve user from existing role");
                return View(list);
            }
            result = await usermanager.AddToRolesAsync(user, list.Where(x => x.IsSelected).Select(x => x.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "cannot add selected user to roles");
                return View(list); 
            }
            return RedirectToAction("EditUser", new {ID=id});
        }

        [HttpGet]
        public async Task<IActionResult> ManageClaim(string id)
        {
            var user = await usermanager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMsg = $"User with ID: {id} not found";
                return View("ViewNotFound");
            }
            var userClaims=await usermanager.GetClaimsAsync(user);
            var model = new ClaimsViewModel();
            model.UserID = id;
            foreach (var item in ClaimStore.AllClaims)
            {
                var obj = new UserClaim();
                obj.ClaimType = item.Type;
                
                if (userClaims.Any(c => c.Type == item.Type && c.Value=="true"))
                {
                    obj.IsSelected = true;
                    
                }
                
                model.Cliam.Add(obj);

                
            }
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> ManageClaim(ClaimsViewModel model)
        {
            var user = await usermanager.FindByIdAsync(model.UserID);
            if(user==null)
            {
                ViewBag.ErrorMsg = $"User with ID: {model.UserID} not found";
                return View("ViewNotFound");
            }

            var claims = await usermanager.GetClaimsAsync(user);
            var result = await usermanager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user form existing claim");
                return View(model);

            }
            result = await usermanager.AddClaimsAsync(user, model.Cliam.Select(c => new Claim ( c.ClaimType, c.IsSelected ?"true":"false" )));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }
            return RedirectToAction("EditUser",new {id=model.UserID});
        }


    }
}
