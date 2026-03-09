using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Constants;
using Tashyeed.Web.Modules.ProjectAssignment.Services;
using Tashyeed.Web.Modules.ProjectAssignment.ViewModels;

namespace Tashyeed.Web.Modules.ProjectAssignment.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class ProjectAssignmentController : Controller
    {
        private readonly IProjectAssignmentService _assignmentService;
        private readonly AppDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectAssignmentController(
            IProjectAssignmentService assignmentService,
            AppDBContext context,
            UserManager<ApplicationUser> userManager)
        {
            _assignmentService = assignmentService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var assignments = await _assignmentService.GetAllAsync();
            return View(assignments);
        }

        public async Task<IActionResult> Assign()
        {
            await PopulateViewBags();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignUserVM vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBags();
                return View(vm);
            }

            var result = await _assignmentService.AssignUserAsync(vm);
            if (!result)
            {
                TempData["Message"] = "هذا الموظف تم تعينه بالفعل في المشروع";
                await PopulateViewBags();
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int assignmentId)
        {
            await _assignmentService.RemoveAssignmentAsync(assignmentId);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateViewBags()
        {
            ViewBag.Projects = _context.Projects
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();

            var userRoles = new Dictionary<string, string>();
            var userSelectList = new List<SelectListItem>();

            var excludedRoles = new[] { RoleNames.AccountingManager, RoleNames.Admin };

            foreach (var user in _userManager.Users.Where(u => u.IsActive).ToList())
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "";

                if (excludedRoles.Contains(role)) continue;

                userRoles[user.Id] = role;
                userSelectList.Add(new SelectListItem
                {
                    Value = user.Id,
                    Text = user.FullName ?? user.Email
                });
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.Users = userSelectList;
        }

    }
}
