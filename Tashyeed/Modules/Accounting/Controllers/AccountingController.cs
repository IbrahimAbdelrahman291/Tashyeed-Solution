using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Constants;
using Tashyeed.Web.Modules.Accounting.Services;

namespace Tashyeed.Web.Modules.Accounting.Controllers
{
    [Authorize(Roles = RoleNames.AccountingManager + "," + RoleNames.Admin)]
    public class AccountingController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IReportService _reportService;

        public AccountingController(AppDBContext context, IReportService reportService)
        {
            _context = context;
            _reportService = reportService;
        }

        public async Task<IActionResult> Projects()
        {
            var projects = await _context.Projects.ToListAsync();
            return View(projects);
        }

        public async Task<IActionResult> Custodies()
        {
            var custodies = await _context.Custodies
                .Include(c => c.Project)
                .Include(c => c.GivenBy)
                .Include(c => c.GivenTo)
                .ToListAsync();
            return View(custodies);
        }

        public async Task<IActionResult> Expenses()
        {
            var expenses = await _context.Expenses
                .Include(e => e.Project)
                .Include(e => e.SubmittedBy)
                .Include(e => e.ApprovedBy)
                .ToListAsync();
            return View(expenses);
        }

        public async Task<IActionResult> Procurement()
        {
            var orders = await _context.PurchaseOrders
                .Include(po => po.Project)
                .Include(po => po.PurchasedBy)
                .Include(po => po.PurchaseRequest)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Workers()
        {
            var workers = await _context.Workers
                .Include(w => w.Project)
                .Include(w => w.DailyAttendances)
                .ToListAsync();
            return View(workers);
        }

        public async Task<IActionResult> MonthlyReport()
        {
            var projects = await _context.Projects.ToListAsync();
            ViewBag.Projects = projects;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectReport(int projectId, int month, int year)
        {
            var report = await _reportService.GetProjectReportAsync(projectId, month, year);
            if (report is null) return NotFound();
            return Json(new
            {
                projectName = report.ProjectName,
                budget = report.Budget,
                custodiesTotal = report.CustodiesTotal,
                expensesTotal = report.ExpensesTotal,
                procurementTotal = report.ProcurementTotal,
                workersTotal = report.WorkersTotal,
                grandTotal = report.GrandTotal,
                remaining = report.Remaining
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetFullReport(int month, int year)
        {
            var report = await _reportService.GetFullReportAsync(month, year);
            return Json(new
            {
                projects = report.Projects.Select(p => new
                {
                    name = p.ProjectName,
                    budget = p.Budget,
                    custodiesTotal = p.CustodiesTotal,
                    expensesTotal = p.ExpensesTotal,
                    procurementTotal = p.ProcurementTotal,
                    workersTotal = p.WorkersTotal,
                    grandTotal = p.GrandTotal
                }),
                grandTotal = report.GrandTotal
            });
        }
    }
}
