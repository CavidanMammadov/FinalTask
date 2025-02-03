using Microsoft.AspNetCore.Mvc;

namespace NinicoFinalTask.Areas.Admin.Controllers
{
    public class DashBoardController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
