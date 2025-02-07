using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Extensions;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Common;
using NinicoFinalTask.ViewModel.Product;

namespace NinicoFinalTask.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(NinicoDbContext _context, IWebHostEnvironment _env) : Controller
    {


        public async Task<IActionResult> Index()
        {
            var datas = await _context.Products.Where(x => x.isDeleted == false).Include(x => x.Category).ToListAsync();
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

            if (vm.OtherImages != null && vm.OtherImages.Any())
            {
                if (!vm.OtherImages.All(x => x.IsValidType("image")))
                {
                    var fileNames = vm.OtherImages.Where(x => !x.IsValidType("image")).Select(x => x.FileName);
                    ModelState.AddModelError("OtherImages", string.Join(",", fileNames) + " an(is) not image");
                }
                if (!vm.OtherImages.All(x => x.IsValidSize(3 * 1024)))
                {
                    var fileNames = vm.OtherImages.Where(x => !x.IsValidSize(3 * 1024)).Select(x => x.FileName);
                    ModelState.AddModelError("OtherImages", string.Join(",", fileNames) + "must be less than 2 mb");

                }
            }
            if (vm.CoverFile != null)
            {
                if (!vm.CoverFile.IsValidType("image"))
                    ModelState.AddModelError("CoverFile", "must be an image");
                if (!vm.CoverFile.IsValidSize(2 * 1024))
                    ModelState.AddModelError("CoverFile", "must be less than 2 mb");
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
                CoverImage = await vm.CoverFile!.UploadAsync(_env.WebRootPath, "imgs", "products"),
                Images = vm.OtherImages.Select(x => new ProductImage
                {
                    ImageUrl = x.UploadAsync(_env.WebRootPath, "imgs", "products").Result
                }).ToList()
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.isDeleted).Select(c => new { c.Id, c.Name }).ToListAsync();
            if (!id.HasValue) return BadRequest();
            var data = await _context.Products.Where(x => x.Id == id.Value).Select(x => new ProductUpdateVM
            {
                CategoryId = x.CategoryId,
                CostPrice = x.CostPrice,
                SellPrice = x.SellPrice,
                Quantity = x.Quantity,
                Discount = x.Discount,
                Name = x.Name,
                Description = x.Description,
                CoverFileUrl = x.CoverImage,
                OtherImagesUrl = x.Images.Select(z => new ImageUrlAndId
                {
                    Url = z.ImageUrl,
                    Id = z.Id
                })

            }).FirstOrDefaultAsync();
            ViewBag.Categories = await _context.Categories.Where(x => !x.isDeleted).Select(c => new { c.Id, c.Name }).ToListAsync();
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, ProductUpdateVM vm)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            if (vm.OtherImages != null && vm.OtherImages.Any())
            {
                var invalidFiles = vm.OtherImages.Where(x => !x.IsValidType("image")).Select(x => x.FileName).ToList();
                if (invalidFiles.Any())
                    ModelState.AddModelError("OtherImages", string.Join(", ", invalidFiles) + " are not images");

                var largeFiles = vm.OtherImages.Where(x => !x.IsValidSize(2 * 1024)).Select(x => x.FileName).ToList();
                if (largeFiles.Any())
                    ModelState.AddModelError("OtherImages", string.Join(", ", largeFiles) + " must be less than 2 MB");
            }

            if (vm.CoverFile != null)
            {
                if (!vm.CoverFile.IsValidType("image"))
                    ModelState.AddModelError("CoverFile", "must be an image");

                if (!vm.CoverFile.IsValidSize(2 * 1024))
                    ModelState.AddModelError("CoverFile", "must be less than 2 MB");
            }


            ViewBag.Categories = await _context.Categories
                .Where(x => !x.isDeleted)
                .Select(c => new { c.Id, c.Name }).ToListAsync();


            if (vm.CoverFile != null)
            {
                if (!string.IsNullOrEmpty(product.CoverImage))
                {
                    var oldCoverPath = Path.Combine(_env.WebRootPath, "imgs", "products", product.CoverImage);
                    if (System.IO.File.Exists(oldCoverPath))
                        System.IO.File.Delete(oldCoverPath);
                }

                product.CoverImage = await vm.CoverFile.UploadAsync(_env.WebRootPath, "imgs", "products");
            }

            if (vm.OtherImagesUrl != null)
            {
                var deletedImageIds = product.Images
                    .Where(img => !vm.OtherImagesUrl.Any(x => x.Id == img.Id))
                    .Select(img => img.Id)
                    .ToList();

                foreach (var img in product.Images.Where(x => deletedImageIds.Contains(x.Id)).ToList())
                {
                    var imgPath = Path.Combine(_env.WebRootPath, "imgs", "products", img.ImageUrl);
                    if (System.IO.File.Exists(imgPath))
                        System.IO.File.Delete(imgPath);

                    _context.ProductImages.Remove(img);
                }
            }

            if (vm.OtherImages != null && vm.OtherImages.Any())
            {
                var newImages = await Task.WhenAll(vm.OtherImages.Select(async x => new ProductImage
                {
                    ImageUrl = await x.UploadAsync(_env.WebRootPath, "imgs", "products")
                }));

                product.Images = product.Images.ToList();
                product.Images.AddRange(newImages);
            }

            product.CategoryId = vm.CategoryId;
            product.CostPrice = vm.CostPrice;
            product.SellPrice = vm.SellPrice;
            product.Name = vm.Name;
            product.Discount = vm.Discount;
            product.Quantity = vm.Quantity;
            product.Description = vm.Description;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return View();
            var data = await _context.Products.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);
            if (data is null) return BadRequest();
            _context.Products.Remove(data);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Hide(int? id)
        {
            if (!id.HasValue) return View();
            var data = await _context.Products.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);
            if (data is null) return BadRequest();
            data.isDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (!id.HasValue) return BadRequest();
            var img = await _context.ProductImages.FindAsync(id.Value);
            if (img == null) return NotFound();
            _context.ProductImages.Remove(img);
            await _context.SaveChangesAsync();
            string path = Path.Combine(_env.WebRootPath, "imgs", "products", img.ImageUrl);

            if (Path.Exists(path))
                System.IO.File.Delete(path);

            return View();
        }

    }
}
