using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Extensions;
using NinicoFinalTask.ViewModel.Slider;

namespace NinicoFinalTask.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController(NinicoDbContext _context, IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var data = await _context.Sliders.ToListAsync();
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
                return View();
            }
            File = await vm.File.UploadAsync(_env.WebRootPath, "imgs", "sliders");
            return View();
        }
    }
}
