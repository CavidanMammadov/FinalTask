using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NinicoFinalTask.Models;
using NinicoFinalTask.Services.Abstracts;
using NinicoFinalTask.ViewModel.Auths;
using System.Text;

namespace NinicoFinalTask.Controllers
{
    public class AccountController(UserManager<User> _userManager, SignInManager<User> _signInManager
        ,IEmailService _service) : Controller
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
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _service.SendEmailConfirmationAsync(user.Email ,user.UserName ,token);
            return Content("Email sent");
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

                ModelState.AddModelError("", "You must enter your email");
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
        public async Task<IActionResult> VerifyEmail(string token , string user)
        {
          var entity =   await _userManager.FindByNameAsync(user);
            if (entity is null) return BadRequest();
            var result = await _userManager.ConfirmEmailAsync(entity, token.Replace(' ','+'));
           if(! result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in result.Errors)
                {
                    sb.AppendLine(item.Description);
                }
                return Content(sb.ToString());
            }
            await _signInManager.SignInAsync(entity, true);
            return RedirectToAction("Index","Home");

        }

    }
}
