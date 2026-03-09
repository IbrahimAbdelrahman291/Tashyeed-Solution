using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Constants;
using Tashyeed.Web.Modules.Workers.Services;
using Tashyeed.Web.Modules.Workers.ViewModels;

namespace Tashyeed.Web.Modules.Workers.Controllers
{
    [Authorize]
    public class WorkersController : Controller
    {
        private readonly IWorkerService _workerService;
        private readonly AppDBContext _context;

        public WorkersController(IWorkerService workerService, AppDBContext context)
        {
            _workerService = workerService;
            _context = context;
        }

        // مدير المشروع والأدمن بيشوفوا طلبات العمالة
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.ProjectManager)]
        public async Task<IActionResult> Requests()
        {
            var requests = await _workerService.GetAllRequestsAsync();

            if (User.IsInRole(RoleNames.ProjectManager))
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

        // المشرف بيطلب عمالة
        [Authorize(Roles = RoleNames.Supervisor)]
        public IActionResult Request()
        {
            PopulateProjectsViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Supervisor)]
        public async Task<IActionResult> Request(WorkerRequestVM vm)
        {
            if (!ModelState.IsValid)
            {
                PopulateProjectsViewBag();
                return View(vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _workerService.CreateRequestAsync(vm, userId);
            return RedirectToAction(nameof(MyWorkers));
        }

        // مدير المشروع بيوافق أو يرفض
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.ProjectManager)]
        public async Task<IActionResult> Approve(int requestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _workerService.ApproveRequestAsync(requestId, userId);
            return RedirectToAction(nameof(Requests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.ProjectManager)]
        public async Task<IActionResult> Reject(int requestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _workerService.RejectRequestAsync(requestId, userId);
            return RedirectToAction(nameof(Requests));
        }

        // المشرف بيشوف العمال بتاعته ويضيف عمال جدد
        [Authorize(Roles = RoleNames.Supervisor)]
        public async Task<IActionResult> MyWorkers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var projectIds = _context.ProjectAssignments
                .Where(pa => pa.UserId == userId)
                .Select(pa => pa.ProjectId)
                .ToList();

            var allWorkers = new List<WorkerListVM>();
            foreach (var projectId in projectIds)
            {
                var workers = await _workerService.GetWorkersByProjectAsync(projectId);
                allWorkers.AddRange(workers);
            }

            return View(allWorkers);
        }

        [Authorize(Roles = RoleNames.Supervisor)]
        public IActionResult AddWorker()
        {
            PopulateProjectsViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Supervisor)]
        public async Task<IActionResult> AddWorker(AddWorkerVM vm)
        {
            if (!ModelState.IsValid)
            {
                PopulateProjectsViewBag();
                return View(vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _workerService.AddWorkerAsync(vm, userId);
            return RedirectToAction(nameof(MyWorkers));
        }

        // الحضور اليومي
        [Authorize(Roles = RoleNames.Supervisor)]
        public IActionResult AddAttendance(int workerId, string workerName)
        {
            var vm = new DailyAttendanceVM
            {
                WorkerId = workerId,
                WorkerName = workerName,
                AttendanceDate = DateOnly.FromDateTime(DateTime.Today)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Supervisor)]
        public async Task<IActionResult> AddAttendance(DailyAttendanceVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var success = await _workerService.AddDailyAttendanceAsync(vm, userId);

            if (success && vm.IsPresent == true)
                TempData["Success"] = "تم تسجيل الحضور بنجاح ✅";
            else if (success && vm.IsPresent == false)
                TempData["Error"] = "لم يتم تسجيل حضور هذا اليوم لعدم تسجيل العامل حاضر في هذا اليوم";
            else
                TempData["Error"] = "العامل متسجل حضور في هذا اليوم بالفعل";
            return RedirectToAction(nameof(MyWorkers));
        }

        // التقرير الشهري
        [Authorize(Roles = RoleNames.Supervisor)]
        public async Task<IActionResult> MonthlyReport(int workerId, string workerName)
        {
            var now = DateTime.Now;
            var existing = await _workerService.GetMonthlyReportAsync(workerId, now.Month, now.Year);

            var vm = existing ?? new MonthlyReportVM
            {
                WorkerId = workerId,
                WorkerName = workerName,
                Month = now.Month,
                Year = now.Year
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Supervisor)]
        public async Task<IActionResult> MonthlyReport(MonthlyReportVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _workerService.SaveMonthlyReportAsync(vm, userId);
            return RedirectToAction(nameof(MyWorkers));
        }
        [Authorize(Roles = RoleNames.Supervisor)]
        public async Task<IActionResult> MyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var requests = await _workerService.GetMyRequestsAsync(userId);
            return View(requests);
        }
        [Authorize(Roles = RoleNames.Supervisor)]
        public async Task<IActionResult> PaymentSummary(int workerId)
        {
            var summary = await _workerService.GetPaymentSummaryAsync(workerId);
            return View(summary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Supervisor)]
        public async Task<IActionResult> PayWorker(int workerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _workerService.PayWorkerAsync(workerId, userId);

            if (!result)
                TempData["Error"] = "مش هتقدر تصرف، تأكد إن عندك عهدة كافية";

            return RedirectToAction(nameof(MyWorkers));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Supervisor)]
        public async Task<IActionResult> ToggleWorker(int workerId)
        {
            await _workerService.ToggleWorkerAsync(workerId);
            return RedirectToAction(nameof(MyWorkers));
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
