using BaseIdentity.Context;
using BaseIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseIdentity.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View(new UserSignInViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(UserSignInViewModel signModel)
        {
            if (ModelState.IsValid)
            {
                var identityResult = await _signInManager.PasswordSignInAsync(signModel.UserName, signModel.Password, signModel.RememberMe, true);
                //yanlış girilmeye karşı hesap kilitleme
                if (identityResult.IsLockedOut)
                {
                    //Hangi kullanıcı için engelenen girilen hesap süresi
                    var values = await _userManager.GetLockoutEndDateAsync(await _userManager.FindByNameAsync(signModel.UserName));
                    var limitedTime = values.Value;

                    var limitedMinute =  limitedTime.Minute - DateTime.Now.Minute; 

                    ModelState.AddModelError("", $"5 kere yanlış girdiğiniz için hesabınız kitlenmiştir.{limitedMinute}  dk kilitlenmiştir ");
                    return View("Index", signModel);
                }
                //Email doğrulama isteği
                if (identityResult.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Email adresinizi lütfen doğrulayınız.");
                    return View("Index", signModel);
                }
                //başarılı ise
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index", "Panel");
                }

                var failCount = await _userManager.GetAccessFailedCountAsync(await _userManager.FindByNameAsync(signModel.UserName));
                ModelState.AddModelError("", $"Kullanıcı adı veya şifre hatalı {5 - failCount} kadar yanlış girerseniz hesabınız bloklanacak");
            }
            return View("Index", signModel);
        }

        public IActionResult SignUp()
        {
            return View(new UserSignUpViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserSignUpViewModel signupModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    Email = signupModel.Email,
                    Name = signupModel.Name,
                    SurName = signupModel.SurName,
                    UserName = signupModel.UserName
                };
                var result = await _userManager.CreateAsync(user, signupModel.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(signupModel);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
