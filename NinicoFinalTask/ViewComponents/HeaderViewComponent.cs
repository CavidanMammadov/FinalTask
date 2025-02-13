using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.ViewModel.Basket;
using System.ComponentModel;
using System.Text.Json;

namespace NinicoFinalTask.ViewComponents
{
    public class HeaderViewComponent(NinicoDbContext _context) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var BasketIds = JsonSerializer.Deserialize<List<BasketProductItemVM>>(Request.Cookies["basket"] ?? "[]");
            var prods = await _context.Products.Where(x => BasketIds.Select(y => y.Id).Any(y => y == x.Id)).Select(x => new GetBasketItemVM
            {
                Id = x.Id,
                Discount = x.Discount,
                ImageUrl = x.CoverImage,
                Name = x.Name,
                SellPrice = x.SellPrice
            }).ToListAsync();
            foreach (var item in prods)
            {
                item.Count = BasketIds!.FirstOrDefault(x => x.Id == item.Id)!.Count;
            }
            return View(prods);
        }
    }
}
