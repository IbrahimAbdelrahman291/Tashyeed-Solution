using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Constants;
using Tashyeed.Web.Modules.CustodyModule.Services;
using Tashyeed.Web.Modules.CustodyModule.ViewModels;

namespace Tashyeed.Web.Modules.CustodyModule.Controllers
{
    [Authorize]
    public class CustodyController : Controller
    {
        private readonly ICustodyService _custodyService;
        private readonly AppDBContext _context;

        public CustodyController(ICustodyService custodyService, AppDBContext context)
        {
            _custodyService = custodyService;
            _context = context;
        }

        // الأدمن ومدير المشروع بيشوفوا كل العهد
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.ProjectManager)]
        public async Task<IActionResult> Index()
        {
            var custodies = await _custodyService.GetAllAsync();
            return View(custodies);
        }

        // المشرف ومدير المشتريات بيشوفوا العهد المعلقة بتاعتهم
        [Authorize(Roles = RoleNames.Supervisor + "," + RoleNames.ProcurementManager)]
        public async Task<IActionResult> MyCustodies()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var custodies = await _custodyService.GetMyPendingCustodiesAsync(userId);
            return View(custodies);
        }

        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.ProjectManager)]
        public IActionResult Assign(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            PopulateViewBags();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.ProjectManager)]
        public async Task<IActionResult> Assign(AssignCustodyVM vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                PopulateViewBags();
                return View(vm);
            }

            var givenByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _custodyService.AssignCustodyAsync(vm, givenByUserId);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Supervisor + "," + RoleNames.ProcurementManager)]
        public async Task<IActionResult> Confirm(int custodyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _custodyService.ConfirmCustodyAsync(custodyId, userId);
            return RedirectToAction(nameof(MyCustodies));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Supervisor + "," + RoleNames.ProcurementManager)]
        public async Task<IActionResult> Reject(int custodyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _custodyService.RejectCustodyAsync(custodyId, userId);
            return RedirectToAction(nameof(MyCustodies));
        }

        private void PopulateViewBags()
        {
            ViewBag.Projects = _context.Projects
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();

            ViewBag.Users = _context.ProjectAssignments
                .Where(pa => pa.Role == RoleNames.ProcurementManager || pa.Role == RoleNames.Supervisor)
                .Select(pa => new SelectListItem
                {
                    Value = pa.UserId,
                    Text = pa.User.FullName ?? pa.User.Email
                })
                .Distinct()
                .ToList();
        }
    }
}

