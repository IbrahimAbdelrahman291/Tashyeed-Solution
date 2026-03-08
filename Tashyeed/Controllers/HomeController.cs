using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tashyeed.Models;

namespace Tashyeed.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
