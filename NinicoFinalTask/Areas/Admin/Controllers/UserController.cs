using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NinicoFinalTask.Helpers;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Auths;

namespace NinicoFinalTask.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =RoleConstant.User)]
    public class UserController(UserManager<User> _userManager,RoleManager<IdentityRole> _roleManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<UserItemVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); 
                userList.Add(new UserItemVM
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(userList);
        }



        [HttpGet]
        public async Task<IActionResult> ChangeRole()
        {
            ViewBag.Users = _userManager.Users.ToList();
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();

            return View(new UserRoleChangeVM());
        }


        [HttpPost]
        public async Task<IActionResult> ChangeRole(string id, string newRole)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");

            if (!await _roleManager.RoleExistsAsync(newRole))
                return BadRequest("Selected role does not exist");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);
            
            return RedirectToAction("Index"); 
        }
        public async Task<IActionResult> DeleteUser(string? id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("User ID is required");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("User could not be deleted");
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
