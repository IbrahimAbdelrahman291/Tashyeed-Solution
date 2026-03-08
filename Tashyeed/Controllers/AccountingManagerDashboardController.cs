using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Constants;
using Tashyeed.Shared.Enums;
using Tashyeed.Web.ViewModels.AccountingDashboard;

namespace Tashyeed.Web.Controllers
{
    [Authorize(Roles = RoleNames.AccountingManager)]
    public class AccountingManagerDashboardController : Controller
    {
        private readonly AppDBContext _context;

        public AccountingManagerDashboardController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new AccountingManagerDashboardVM
            {
                TotalProjects = await _context.Projects.CountAsync(),

                TotalSpent = await _context.Projects.SumAsync(p => p.SpentAmount),

                TotalCustodies = await _context.Custodies
                    .Where(c => c.Status == CustodyStatus.Confirmed)
                    .SumAsync(c => c.Amount),

                TotalProcurement = await _context.PurchaseOrders
                    .SumAsync(po => po.Amount),

                TotalExpenses = await _context.Expenses
                    .Where(e => e.Status == RequestStatus.Approved)
                    .SumAsync(e => e.Amount),

                TotalWorkerPayments = await _context.DailyAttendances
                    .Where(da => da.IsPresent && da.IsPaid)
                    .Include(da => da.Worker)
                    .SumAsync(da => da.Worker.DailyRate + (da.OvertimeHours * da.Worker.OvertimeHourRate))
            };

            return View(vm);
        }
    }
}