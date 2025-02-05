using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Common;
using NinicoFinalTask.ViewModel.Product;
using NinicoFinalTask.ViewModel.Slider;
using System.Diagnostics;

namespace NinicoFinalTask.Controllers
{
    public class HomeController(NinicoDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            HomeVM vm = new();
            vm.Sliders = await _context.Sliders.Where(x => x.isDeleted == false).Select(x => new SliderItemVM
            {
                Title = x.Title,
                SubTitle = x.SubTitle,
                Link = x.Link,
                StartPrice = x.StartPrice,
                ImageUrl = x.ImageUrl
            }).ToListAsync();
            vm.Products = await _context.Products.Where(x => x.isDeleted == false).Select(x => new ProductItemVM
            {
                Discount = x.Discount,
                Id = x.Id,
                ImageUrl = x.CoverImage,
                IsInStock = x.Quantity> 0,
                Name = x.Name,
                Price = x.SellPrice

            }).ToListAsync();
            return View(vm);
        }


    }
}
