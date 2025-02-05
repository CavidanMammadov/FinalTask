using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Extensions;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Product;

namespace NinicoFinalTask.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(NinicoDbContext _context, IWebHostEnvironment _env) : Controller
    {
       

        public async Task<IActionResult> Index()
        {
            var datas = await _context.Products.Where(x => x.isDeleted == false).Include( x=> x.Category).ToListAsync();
            return View(datas);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.isDeleted).Select(c => new { c.Id, c.Name }).ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (vm.CoverFile != null)
            {
                if (!vm.CoverFile.IsValidType("image"))
                    ModelState.AddModelError("CoverFile", "must be an image");
                if (!vm.CoverFile.IsValidSize(1024))
                    ModelState.AddModelError("CoverFile", "must be less than 1 mb");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.Where(x => !x.isDeleted).Select(c => new { c.Id, c.Name }).ToListAsync();
                return View();
            }
            Product product = new Product
            {
                CategoryId = vm.CategoryId,
                CostPrice = vm.CostPrice,
                SellPrice = vm.SellPrice,
                Name = vm.Name,
                Discount = vm.Discount,
                Quantity = vm.Quantity,
                Description = vm.Description,
                CoverImage = await vm.CoverFile!.UploadAsync(_env.WebRootPath, "imgs", "products")
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //    [HttpGet]
        //    public async  Task<IActionResult> Update()
        //    {
        //        return View();
        //    }
        //    [HttpPost]
        //    public async  Task<IActionResult> Update()
        //    {
        //        return View();
        //    }
        //    public async  Task<IActionResult> Delete()
        //    {
        //        return View();
        //    }
    }
}
