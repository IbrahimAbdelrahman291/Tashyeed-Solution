using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tashyeed.Shared.Constants;

namespace Tashyeed.Web.Controllers
{
    [Authorize(Roles = RoleNames.Engineer)]
    public class EngineerDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
