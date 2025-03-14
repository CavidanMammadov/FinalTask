using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Models;

namespace NinicoFinalTask.Controllers
{
    public class WishListController(NinicoDbContext _context, UserManager<User> _userManager) : Controller
    {
        public async Task<IActionResult> Wishlist()
        {
            string userId = _userManager.GetUserId(User);
            var wishlist = await _context.WishLists
            .Where(w => w.UserId == userId)
            .Include(w => w.Product)
            .ToListAsync();
            return View(wishlist);
        }
        public async Task<IActionResult> AddProduct(int Id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            string userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Json("İstifadəçi tapılmadı!");
            }
            if (!_context.WishLists.Any(x => x.UserId == userId && x.ProductId == Id))
            {
                var wishlistItem = new WishList
                {
                    UserId = userId,
                    ProductId = Id
                };

                _context.WishLists.Add(wishlistItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Wishlist", "Wishlist");
        }
        public async Task<IActionResult> Remove(int Id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            string userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Json("İstifadəçi tapılmadı!");
            }

            var wishlistItem = _context.WishLists.FirstOrDefault(x => x.UserId == userId && x.ProductId == Id);

            if (wishlistItem != null)
            {
                _context.WishLists.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Wishlist", "Wishlist");
        }

    }
}
