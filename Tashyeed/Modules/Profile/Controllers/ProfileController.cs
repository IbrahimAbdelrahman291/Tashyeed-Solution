using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Web.Modules.Profile.ViewModels;

namespace Tashyeed.Web.Modules.Profile.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDBContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, AppDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "-";

            var assignment = await _context.ProjectAssignments
                .Include(pa => pa.Project)
                .FirstOrDefaultAsync(pa => pa.UserId == user.Id);

            var vm = new ProfileVM
            {
                FullName = user.FullName ?? user.Email,
                Email = user.Email,
                Role = role,
                ProjectName = assignment?.Project?.Name
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "من فضلك تأكد من البيانات المدخلة";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(user, vm.CurrentPassword, vm.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "تم تغيير الباسورد بنجاح ✓";
            }
            else
            {
                TempData["Error"] = "الباسورد الحالي غلط";
            }

            return RedirectToAction("Index");
        }
    }
}
