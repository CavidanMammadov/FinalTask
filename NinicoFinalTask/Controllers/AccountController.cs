using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Auths;

namespace NinicoFinalTask.Controllers
{
    public class AccountController(UserManager<User> _userManager, SignInManager<User> _signInManager) : Controller
    {
        bool isAuthendicated => User.Identity?.IsAuthenticated ?? false;
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserCreateVM vm)
        {
            if(isAuthendicated == true) return RedirectToAction("Index","Home");
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
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm, string? ReturnUrl)
        {
            if (!ModelState.IsValid) return View();

            User? user = null;
            if (vm.UsernameOrEmail.Contains('@'))
                user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
            else
                user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "USERNAME OR PASSWORD IS INCORRECT");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);

            if (!result.Succeeded)
            {
                if (result.IsNotAllowed)
                    ModelState.AddModelError("", "Your account is not allowed to sign in.");

                if (result.IsLockedOut && user.LockoutEnd.HasValue)
                    ModelState.AddModelError("", "Wait until " + user.LockoutEnd.Value.ToString("yyyy-MM-dd HH:mm:ss"));

                ModelState.AddModelError("", "USERNAME OR PASSWORD IS INCORRECT");
                return View();
            }

            if (string.IsNullOrWhiteSpace(ReturnUrl))
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", "DashBoard", new { Area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return LocalRedirect(ReturnUrl);
        }
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

    }
}
