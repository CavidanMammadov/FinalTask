using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Auths;

namespace NinicoFinalTask.Controllers
{
    public class AccountController(UserManager<User> _userManager) : Controller
    {
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserCreateVM vm)
        {
            if (!ModelState.IsValid) return View();
            return View();
            User user = new User
            {
                Email = vm.Email,
                FullName = vm.FullName,
                UserName = vm.UserName,
                ProfileImageUrl ="photo.jpg"
            };
            var result = await _userManager.CreateAsync(user,vm.Password);
            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return RedirectToAction("Index","Home");
        }



        public async Task<IActionResult> Login()
        {
            return View();
        }
    }
}
