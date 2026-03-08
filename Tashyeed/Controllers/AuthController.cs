using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Shared.Constants;
using Tashyeed.Web.ViewModels.Auth;

namespace Tashyeed.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                // نتأكد إن اليوزر مش معطل
                if (!user!.IsActive)
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "حسابك معطل، تواصل مع الأدمن");
                    return View(model);
                }
                var roles = await _userManager.GetRolesAsync(user!);

                if (roles.Contains(RoleNames.Admin))
                    return RedirectToAction("Index", "AdminDashboard");
                else if (roles.Contains(RoleNames.ProjectManager))
                    return RedirectToAction("Index", "ProjectManagerDashboard");
                else if (roles.Contains(RoleNames.ProcurementManager))
                    return RedirectToAction("Index", "ProcurementManagerDashboard");
                else if (roles.Contains(RoleNames.Supervisor))
                    return RedirectToAction("Index", "SupervisorDashboard");
                else if (roles.Contains(RoleNames.Engineer))
                    return RedirectToAction("Index", "EngineerDashboard");
                else if (roles.Contains(RoleNames.AccountingManager))
                    return RedirectToAction("Index", "AccountingManagerDashboard");

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور خطأ");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}