using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Constants;
using Tashyeed.Shared.Enums;
using Tashyeed.Web.ViewModels.AdminDashboard;

namespace Tashyeed.Web.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class AdminDashboardController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDashboardController(AppDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var year = DateTime.Now.Year;

            var monthlyCustodies = new List<decimal>();
            var monthlyExpenses = new List<decimal>();
            var monthlyProcurement = new List<decimal>();
            var monthlyWorkers = new List<decimal>();

            for (int month = 1; month <= 12; month++)
            {
                monthlyCustodies.Add(await _context.Custodies
                    .Where(c => c.CreatedAt.Month == month
                        && c.CreatedAt.Year == year
                        && c.Status == CustodyStatus.Confirmed)
                    .SumAsync(c => c.Amount));

                monthlyExpenses.Add(await _context.Expenses
                    .Where(e => e.CreatedAt.Month == month
                        && e.CreatedAt.Year == year
                        && e.Status == RequestStatus.Approved)
                    .SumAsync(e => e.Amount));

                monthlyProcurement.Add(await _context.PurchaseOrders
                    .Where(po => po.CreatedAt.Month == month
                        && po.CreatedAt.Year == year)
                    .SumAsync(po => po.Amount));

                var workers = await _context.Workers
                    .Include(w => w.DailyAttendances)
                    .ToListAsync();

                decimal workersTotal = 0;
                foreach (var w in workers)
                {
                    var paidDays = w.DailyAttendances
                        .Where(da => da.IsPresent && da.IsPaid
                            && da.PaidAt.HasValue
                            && da.PaidAt.Value.Month == month
                            && da.PaidAt.Value.Year == year)
                        .ToList();
                    workersTotal += (paidDays.Count * w.DailyRate) +
                                    (paidDays.Sum(da => da.OvertimeHours) * w.OvertimeHourRate);
                }
                monthlyWorkers.Add(workersTotal);
            }

            var vm = new AdminDashboardVM
            {
                TotalProjects = await _context.Projects.CountAsync(),
                ActiveProjects = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.Active),
                OnHoldProjects = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.OnHold),
                CompletedProjects = await _context.Projects.CountAsync(p => p.Status == ProjectStatus.Completed),
                TotalUsers = _userManager.Users.Count(),
                ActiveUsers = _userManager.Users.Count(u => u.IsActive),
                MonthlyCustodies = monthlyCustodies,
                MonthlyExpenses = monthlyExpenses,
                MonthlyProcurement = monthlyProcurement,
                MonthlyWorkers = monthlyWorkers
            };

            return View(vm);
        }
    }
}
