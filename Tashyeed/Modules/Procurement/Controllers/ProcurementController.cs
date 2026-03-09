using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Constants;
using Tashyeed.Web.Modules.Procurement.Services;
using Tashyeed.Web.Modules.Procurement.ViewModels;

namespace Tashyeed.Web.Modules.Procurement.Controllers
{
    [Authorize]
    public class ProcurementController : Controller
    {
        private readonly IProcurementService _procurementService;
        private readonly AppDBContext _context;

        public ProcurementController(IProcurementService procurementService, AppDBContext context)
        {
            _procurementService = procurementService;
            _context = context;
        }

        // الأدمن ومدير المشروع ومدير المشتريات بيشوفوا كل الطلبات
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.ProjectManager + "," + RoleNames.ProcurementManager)]
        public async Task<IActionResult> Index()
        {
            var requests = await _procurementService.GetAllAsync();

            if (User.IsInRole(RoleNames.ProcurementManager))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var projectIds = _context.ProjectAssignments
                    .Where(pa => pa.UserId == userId)
                    .Select(pa => pa.ProjectId)
                    .ToList();

                requests = requests.Where(r => projectIds.Contains(r.ProjectId));
            }

            return View(requests);
        }

        // المهندس بيشوف طلباته بس
        [Authorize(Roles = RoleNames.Engineer)]
        public async Task<IActionResult> MyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var allRequests = await _procurementService.GetAllAsync();
            return View(allRequests.Where(r => r.RequestedById == userId));
        }

        // المهندس بيرفع طلب جديد
        [Authorize(Roles = RoleNames.Engineer)]
        public IActionResult Request()
        {
            PopulateProjectsViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Engineer)]
        public async Task<IActionResult> Request(PurchaseRequestVM vm)
        {
            if (!ModelState.IsValid)
            {
                PopulateProjectsViewBag();
                return View(vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _procurementService.CreateRequestAsync(vm, userId);
            return RedirectToAction(nameof(MyRequests));
        }

        // مدير المشتريات بيرفع فاتورة الشراء
        [Authorize(Roles = RoleNames.ProcurementManager)]
        public IActionResult Order(int requestId, int projectId)
        {
            var vm = new PurchaseOrderVM
            {
                PurchaseRequestId = requestId,
                ProjectId = projectId
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.ProcurementManager)]
        public async Task<IActionResult> Order(PurchaseOrderVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _procurementService.CreateOrderAsync(vm, userId);

            if (!result)
            {
                ModelState.AddModelError(string.Empty,
                    "مش هتقدر تنفذ الطلب، رصيد عهدتك مش كافي للمبلغ ده");
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Engineer)]
        public async Task<IActionResult> DeleteRequest(int requestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _procurementService.DeleteRequestAsync(requestId, userId);

            if (!result)
                TempData["Error"] = "مش هتقدر تمسح الطلب ده";
            else
                TempData["Success"] = "تم مسح الطلب بنجاح ✓";

            return RedirectToAction(nameof(MyRequests));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.ProcurementManager)]
        public async Task<IActionResult> RevertOrder(int requestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _procurementService.RevertOrderAsync(requestId, userId);

            if (!result)
                TempData["Error"] = "مش هتقدر تلغي الشراء ده";
            else
                TempData["Success"] = "تم إلغاء الشراء بنجاح ✓";

            return RedirectToAction(nameof(Index));
        }

        private void PopulateProjectsViewBag()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            ViewBag.Projects = _context.ProjectAssignments
                .Where(pa => pa.UserId == userId)
                .Select(pa => new SelectListItem
                {
                    Value = pa.ProjectId.ToString(),
                    Text = pa.Project.Name
                }).ToList();
        }
    }
}
