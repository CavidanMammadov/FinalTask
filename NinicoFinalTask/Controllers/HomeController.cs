using Microsoft.AspNetCore.Mvc;
using NinicoFinalTask.Models;
using System.Diagnostics;

namespace NinicoFinalTask.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


    }
}
