using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.ViewModel.Basket;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace NinicoFinalTask.Controllers
{
    public class BasketController(NinicoDbContext _context) : Controller
    {
        public async Task<IActionResult> Card()
        {
            return View();
        }
        public async Task<IActionResult> AddBasket(int id)
        {
            var BasketItems = JsonSerializer.Deserialize<List<BasketProductItemVM>>(Request.Cookies["basket"] ?? "[]");
            var item = BasketItems.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                item = new BasketProductItemVM
                {
                    Id = id,
                    Count = 0
                };
                BasketItems.Add(item);
            }
            item.Count++;
            Response.Cookies.Append("basket", JsonSerializer.Serialize(BasketItems));
            return Ok();
        }
       
        public async Task<IActionResult> Delete(int id)
        {
            var basketJson = Request.Cookies["basket"];
            var basketItems = string.IsNullOrEmpty(basketJson)
                ? new List<BasketProductItemVM>()
                : JsonSerializer.Deserialize<List<BasketProductItemVM>>(basketJson);

            var item = basketItems.FirstOrDefault(x => x.Id == id);
            if (item == null) return Json(new { success = false });

            item.Count--;

            if (item.Count <= 0)
            {
                basketItems.Remove(item); // Əgər sayı 0 və ya mənfidirsə, siyahıdan sil
            }

            // Yenilənmiş səbəti cookie-yə yaz
            Response.Cookies.Append("basket", JsonSerializer.Serialize(basketItems), new CookieOptions { Expires = DateTime.Now.AddDays(7) });

            return RedirectToAction("Index","Home");
        }







        public async Task<IActionResult> ClearBasket(int id)
        {
            if (!await _context.Products.AnyAsync(x => x.Id == id)) return NotFound();
            var basketItems = JsonSerializer.Deserialize<List<BasketProductItemVM>>(Request.Cookies["basket"] ?? "[]");

            var item = basketItems.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                item = new BasketProductItemVM
                {
                    Id = id,
                    Count = 0
                };
                basketItems.Add(item);

            }
            item.Count--;
            Response.Cookies.Delete("basket");

            return RedirectToAction("Index", "Home");
        }


    }
}
