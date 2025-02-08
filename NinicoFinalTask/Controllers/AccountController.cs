using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Auths;

namespace NinicoFinalTask.Controllers
{
    public class AccountController(UserManager<User> _userManager,SignInManager<User> _signInManager) : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserCreateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (vm.Password != vm.RePassword)
            {
                ModelState.AddModelError("", "Password and Repassword do not match");
                return View(vm);
            }

            User user = new User
            {
                Email = vm.Email,
                FullName = vm.FullName,
                UserName = vm.UserName,
                ProfileImageUrl = "photo.jpg"
            };

            var result = await _userManager.CreateAsync(user, vm.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(vm);
            }

            return RedirectToAction("Login", "Account");
        }



        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            User? user = null;
            if (vm.UsernameOrEmail.Contains('@'))
                user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
            else
                user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);
            await _signInManager.PasswordSignInAsync(user, vm.Password,vm.RememberMe,true);
            return RedirectToAction("Index", "Home");
        }
    }
}
