using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Extensions;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Slider;
using System.Security.Cryptography;

namespace NinicoFinalTask.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SliderController(NinicoDbContext _context, IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var data = await _context.Sliders.Where(x=> x.isDeleted == false).ToListAsync();
            return View(data);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SliderCreateVm vm)
        {
            if (!ModelState.IsValid) return View();
            if (!vm.File.IsValidType("image"))
            {
                ModelState.AddModelError("File", "File type must be image");
                return View();
            }
            if (!vm.File.IsValidSize(1024))
            {
                ModelState.AddModelError("File", "must be less than 1 mb");
                return RedirectToAction(nameof(Index));
            }

            Slider slider = new Slider
            {
                ImageUrl = await vm.File.UploadAsync(_env.WebRootPath, "imgs", "sliders"),
                Link = vm.Link,
                Title = vm.Title,
                SubTitle = vm.SubTitle,
                StartPrice = vm.StartPrice
            };
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (!id.HasValue) return BadRequest();
            var data = await _context.Sliders.Where(x => x.isDeleted == false).FirstOrDefaultAsync(x => x.Id == id);
            if (data is null) return BadRequest();
            SliderUpdateVM vm = new();
            data.Title = vm.Title;
            data.SubTitle = vm.SubTitle;
            data.StartPrice = vm.StartPrice;
            data.Link = vm.Link;
            return View(vm);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, SliderUpdateVM vm)
        {
            if (!vm.File.IsValidType("image"))
                ModelState.AddModelError("File", "must be an image");
            if (!vm.File.IsValidSize(1024))
                ModelState.AddModelError("FIle", "must be less than 1 mb");
            if (!ModelState.IsValid) return View();
            var data = await _context.Sliders.FindAsync(id);
            if (data is null) return View();
            data.Title = vm.Title;
            data.SubTitle = vm.SubTitle;
            data.Link = vm.Link;
            data.StartPrice = vm.StartPrice;
            data.ImageUrl = await vm.File.UploadAsync(_env.WebRootPath, "imgs", "sliders");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return BadRequest();
            var data = await _context.Sliders.FindAsync(id);
            if (data is null) return BadRequest();
            data.isDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
