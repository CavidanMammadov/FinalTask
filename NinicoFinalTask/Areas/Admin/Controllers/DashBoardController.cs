using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NinicoFinalTask.Helpers;

namespace NinicoFinalTask.Areas.Admin.Controllers
{
    public class DashBoardController : Controller
    {
        [Area("Admin")]
        [Authorize(Roles =RoleConstant.Dashboard)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
