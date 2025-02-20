using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Blog;
using NinicoFinalTask.ViewModel.Category;
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
            vm.Blogs = await _context.Blogs.Where(x => x.isDeleted == false).Select(x => new BlogItemVM
            {
                Title = x.Title,
                Subtitle = x.SubTitle,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                CreatedTime = x.CreatedTime
            }).ToListAsync();
            vm.Categories = await _context.Categories.Where(x => x.isDeleted == false).Select(x => new CategoryItemVM
            {
                Name = x.Name,
            }).ToListAsync();
            return View(vm);
        }
       
        public async Task<IActionResult> About()
        {
            return View();
        } 
        public async Task<IActionResult> AccesDenied()
        {
            return View();
        }

    }
}
