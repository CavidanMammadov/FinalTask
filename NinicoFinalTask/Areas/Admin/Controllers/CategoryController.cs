using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Helpers;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Category;

namespace NinicoFinalTask.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =RoleConstant.Category)]
    public class CategoryController(NinicoDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var datas = await _context.Categories.Where(x => x.isDeleted == false).Include(x => x.Products).ToListAsync();
            return View(datas);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM vm)
        {
            if (vm.Name == null) { ModelState.AddModelError("Name", " ad bos ola bilmez"); }
            if (ModelState.IsValid)
            {
                Category categories = new() { Name = vm.Name };
                await _context.Categories.AddAsync(categories);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));


        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            Category? data = await _context.Categories.FindAsync(id);
            CategoryCreateVM vm = new();
            vm.Name = data.Name;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, CategoryUpdateVM vm)
        {
            if (!ModelState.IsValid) return BadRequest();
            var data = await _context.Categories.FindAsync(id);
            if (data is null) return View();
            data.Name = vm.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return BadRequest();
            var data = await _context.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
            if (data is null) return BadRequest();
            _context.Categories.Remove(data);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Hide(int? id)
        {
            if (!id.HasValue) return BadRequest();
            var data = await _context.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
            if (data is null) return BadRequest();
            data.isDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}

