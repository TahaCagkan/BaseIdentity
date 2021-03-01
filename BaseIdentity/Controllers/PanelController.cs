using BaseIdentity.Context;
using BaseIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BaseIdentity.Controllers
{
    [Authorize]
    public class PanelController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public PanelController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return View(user);
        }

        public async Task<IActionResult> UpdateUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            UserUpdateViewModel model = new UserUpdateViewModel
            {
                Email = user.Email,
                Name = user.Name,
                SurName = user.SurName,
                PhoneNumber = user.PhoneNumber,
                PictureUrl = user.PictureUrl
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserUpdateViewModel updateModel)
        {
            if(ModelState.IsValid)
            {
                //Kullanıcı bilgilerini aldık
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                //resim seçildi ise ugylanan işlemler
                if(updateModel.Picture !=null)
                {
                    var appWorkPlace = Directory.GetCurrentDirectory();
                    var path = Path.GetExtension(updateModel.Picture.FileName);
                    var pictureName = Guid.NewGuid() + path;
                    var savePlace = appWorkPlace + "/wwwroot/img/" + pictureName;

                    using var stream = new FileStream(savePlace, FileMode.Create);
                    await updateModel.Picture.CopyToAsync(stream);
                    user.PictureUrl = pictureName;
                }
                //kullanıcı güncelleme
                user.Name = updateModel.Name;
                user.SurName = updateModel.SurName;
                user.Email = updateModel.Email;
                user.PhoneNumber = updateModel.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if(result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(updateModel);
        }
        [AllowAnonymous]
        public IActionResult EveryoneReach()
        {
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
          await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
    }
}
