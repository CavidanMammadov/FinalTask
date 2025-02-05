using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Slider;
using System.Diagnostics;

namespace NinicoFinalTask.Controllers
{
    public class HomeController(NinicoDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var datas = await _context.Sliders.Where(x => x.isDeleted == false).Select(x => new SliderItemVM
            {
                Title = x.Title,
                SubTitle = x.SubTitle,
                Link = x.Link,
                StartPrice  =x.StartPrice,
                ImageUrl = x.ImageUrl
            }
            ).ToListAsync();
            return View(datas);
        }


    }
}
