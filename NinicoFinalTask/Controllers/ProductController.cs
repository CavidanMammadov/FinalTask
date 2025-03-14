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
            IQueryable<Product> query = _context.Products.Where(x => !x.isDeleted);
            ProductIndexVM vm = new ProductIndexVM
            {
                Products = await query.Select(x => new ProductItemVM
                {
                    IsInStock = x.Quantity > 0,
                    Discount = x.Discount,
                    Name = x.Name,
                    ImageUrl = x.CoverImage,
                    Price = x.SellPrice,
                    Id = x.Id


                }).ToListAsync(),
                Categories = [new CategoryAndCount { Id = 0, Count = await query.CountAsync(), Name = "All" }]

            };
            var cats = await _context.Categories.Where(x => !x.isDeleted).Select(x => new CategoryAndCount
            {
                Name = x.Name,
                Id = x.Id,
                Count = x.Products.Count()
            }).ToListAsync();
            vm.Categories.AddRange(cats);
            return View(vm);

        }
        [HttpGet]
        public async Task<IActionResult> Filter(int? catId = 0, string? price = null, int? minPrice = 10, int? maxPrice = 500)
        {
            if (!catId.HasValue) return BadRequest();
            var query = _context.Products.Where(x => !x.isDeleted && x.SellPrice >= minPrice && x.SellPrice <= maxPrice
            );
            if (catId != 0)
            {

                query = query.Where(x => x.CategoryId == catId);
            }
            var data = await query.Select(x => new ProductItemVM
            {
                IsInStock = x.Quantity > 0,
                Discount = x.Discount,
                Name = x.Name,
                ImageUrl = x.CoverImage,
                Price = x.SellPrice,
                Id = x.Id


            }).ToListAsync();
            return PartialView("_ProductPartial", data);
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

            var productImages =await _context.ProductImages
                .Where(x => x.ProductId == id)
                .Select(x => x.ImageUrl)
                .ToListAsync();
      

            var model = new ProductDetailVM
            {
                Id = data.Id,
                Name = data.Name,
                Description = data.Description,
                Price = data.SellPrice,
                ImageUrl = data.CoverImage,
                CategoryId = data.CategoryId,
                RelatedProducts = relatedProducts,
                IsInStock = data.Quantity>0,
                Discount =data.Discount,
                CategoryName = data.Category!.Name
            };
            model.OtherImagesUrl = productImages;

            return View(model);
        }
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new { message = "Axtarış sorğusu boş ola bilməz!" });
            }

            var products = await _context.Products
                .Where(p => p.Name.Contains(query))
                .Select(p => new { p.Id, p.Name }) 
                .ToListAsync();

            if (!products.Any())
            {
                return Json(new { message = "Uyğun məhsul tapılmadı!" });
            }

            return Json(products);
        }


    }
}
