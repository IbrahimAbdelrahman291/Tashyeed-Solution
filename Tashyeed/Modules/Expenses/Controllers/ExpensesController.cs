using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Constants;
using Tashyeed.Shared.Enums;
using Tashyeed.Web.Modules.Expenses.Services;
using Tashyeed.Web.Modules.Expenses.ViewModels;

namespace Tashyeed.Web.Modules.Expenses.Controllers
{
    [Authorize]
    public class ExpensesController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly AppDBContext _context;

        public ExpensesController(IExpenseService expenseService, AppDBContext context)
        {
            _expenseService = expenseService;
            _context = context;
        }

        // مدير المشتريات والأدمن ومدير المشروع بيشوفوا كل المصاريف
        [Authorize(Roles = RoleNames.Admin + "," + RoleNames.ProjectManager + "," + RoleNames.ProcurementManager)]
        public async Task<IActionResult> Index()
        {
            var expenses = await _expenseService.GetAllAsync();
            return View(expenses);
        }

        // المشرف بيشوف مصاريفه بس
        [Authorize(Roles = RoleNames.Supervisor)]
        public async Task<IActionResult> MyExpenses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var expenses = await _expenseService.GetMyExpensesAsync(userId);
            return View(expenses);
        }

        // المشرف ومدير المشتريات بيضيفوا مصاريف
        [Authorize(Roles = RoleNames.Supervisor + "," + RoleNames.ProcurementManager)]
        public IActionResult Add()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // نتأكد إن عنده عهدة مؤكدة
            var hasCustody = _context.Custodies
                .Any(c => c.GivenToUserId == userId
                    && c.Status == CustodyStatus.Confirmed
                    && c.RemainingAmount > 0);

            if (!hasCustody)
            {
                TempData["Error"] = "عهدتك لا تسمح بتغطية هذا المصروف";
                return RedirectToAction(User.IsInRole(RoleNames.Supervisor)
                    ? "Index"
                    : "Index",
                    "SupervisorDashboard");
            }

            PopulateViewBags();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Supervisor + "," + RoleNames.ProcurementManager)]
        public async Task<IActionResult> Add(AddExpenseVM vm)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewBags();
                return View(vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // تأكيد تاني إن العهدة لسه موجودة
            var hasCustody = _context.Custodies
                .Any(c => c.GivenToUserId == userId
                    && c.Status == CustodyStatus.Confirmed
                    && c.RemainingAmount > 0
                    && c.RemainingAmount > vm.Amount
                    );

            if (!hasCustody)
            {
                TempData["Error"] = "عهدتك لا تسمح بتغطية هذا المصروف";
                PopulateViewBags();
                return View(vm);
            }

            await _expenseService.AddExpenseAsync(vm, userId);

            if (User.IsInRole(RoleNames.Supervisor))
                return RedirectToAction(nameof(MyExpenses));

            return RedirectToAction(nameof(Index));
        }

        // مدير المشتريات بيوافق أو يرفض
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.ProcurementManager)]
        public async Task<IActionResult> Approve(int expenseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _expenseService.ApproveExpenseAsync(expenseId, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.ProcurementManager)]
        public async Task<IActionResult> Reject(int expenseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _expenseService.RejectExpenseAsync(expenseId, userId);
            return RedirectToAction(nameof(Index));
        }

        private void PopulateViewBags()
        {
            // المشرف بيشوف Food بس، مدير المشتريات بيشوف Transportation بس
            if (User.IsInRole(RoleNames.Supervisor))
            {
                ViewBag.ExpenseTypes = new List<SelectListItem>
                {
                    new SelectListItem { Value = ((int)ExpenseType.Food).ToString(), Text = "وجبات" },
                    new SelectListItem { Value = ((int)ExpenseType.housing).ToString(), Text = "سكن" }
                };
            }
            else
            {
                ViewBag.ExpenseTypes = new List<SelectListItem>
                {
                    new SelectListItem { Value = ((int)ExpenseType.Transportation).ToString(), Text = "موصلات" }
                };
            }

            // بيجيب المشاريع المعين عليها الشخص ده بس
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
