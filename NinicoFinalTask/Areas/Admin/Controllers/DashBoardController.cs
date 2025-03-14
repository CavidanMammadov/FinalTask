using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Helpers;

namespace NinicoFinalTask.Areas.Admin.Controllers
{
    public class DashBoardController(NinicoDbContext _context) : Controller
    {
        [Area("Admin")]
        [Authorize(Roles =RoleConstant.Dashboard)]
        public IActionResult Index()
        {
            var orders = _context.Orders
        .Where(o => o.PaymentStatus == "Paid") 
        .ToList();

            return View(orders);
        }
    }
}
