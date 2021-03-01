using BaseIdentity.Context;
using BaseIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseIdentity.Controllers
{
    [Authorize]
    public class RolController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RolController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View(_roleManager.Roles.ToList());
        }

        public IActionResult AddRole()
        {
            return View(new RoleViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                AppRole role = new AppRole
                {
                    Name = roleModel.Name
                };
                var identityResult = await _roleManager.CreateAsync(role);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(roleModel);
        }

        public IActionResult UpdateRole(int id)
        {
            var role = _roleManager.Roles.FirstOrDefault(x => x.Id == id);

            RoleUpdateViewModel model = new RoleUpdateViewModel
            {
                Id = role.Id,
                Name = role.Name
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRole(RoleUpdateViewModel roleUpdateModel)
        {
            var tobeUpdatedRole = _roleManager.Roles.Where(I => I.Id == roleUpdateModel.Id).FirstOrDefault();

            tobeUpdatedRole.Name = roleUpdateModel.Name;

            var identityResult = await _roleManager.UpdateAsync(tobeUpdatedRole);

            if (identityResult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(roleUpdateModel);
        }

        public async Task<IActionResult> DeleteRole(int id)
        {
            var tobeDeletedRole = _roleManager.Roles.FirstOrDefault(x => x.Id == id);

            var identityResult = await _roleManager.DeleteAsync(tobeDeletedRole);
            if (identityResult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Errors"] = identityResult.Errors;
                return RedirectToAction("Index");
            }
        }
        public IActionResult UserList()
        {
            return View(_userManager.Users.ToList());
        }
        public async Task<IActionResult> AssignRole(int id)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);

            TempData["UserId"] = user.Id;

            var roles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            List<RoleAssignViewModel> models = new List<RoleAssignViewModel>();

            foreach (var item in roles)
            {
                RoleAssignViewModel model = new RoleAssignViewModel();
                model.RoleId = item.Id;
                model.Name = item.Name;
                model.Exists = userRoles.Contains(item.Name);
                models.Add(model);
            }
            return View(models);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(List<RoleAssignViewModel> models)
        {
            var userId = (int)TempData["UserId"];
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);

            foreach (var item in models)
            {
                if (item.Exists)
                {
                    await _userManager.AddToRoleAsync(user, item.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, item.Name);
                }
            }

            return RedirectToAction("UserList");
        }


    }
}
