using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Product;

namespace NinicoFinalTask.Controllers
{
    public class ProductController(NinicoDbContext _context, IWebHostEnvironment _env) :Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int? id)
        {
            var data = _context.Products
                .Include(p => p.Category)
                .Include(x=> x.Images)
                .FirstOrDefault(p => p.Id == id);

            if (data == null)
                return NotFound();

            var relatedProducts = _context.Products
                .Where(x => x.CategoryId == data.CategoryId && x.Id != id)
                .Take(4)
                .Select(x => new ProductItemVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.SellPrice,
                    ImageUrl = x.CoverImage
                }).ToList();

            // Məhsulun əlavə şəkillərini bazadan çəkirik
            var productImages = _context.ProductImages
                .Where(x => x.ProductId == id)
                .Select(x => x.ImageUrl)
                .ToList();

            var model = new ProductDetailVM
            {
                Id = data.Id,
                Name = data.Name,
                Description = data.Description,
                Price = data.SellPrice,
                ImageUrl = data.CoverImage,
                CategoryId = data.CategoryId,
                RelatedProducts = relatedProducts
            };

            return View(model);
        }



    }
}
