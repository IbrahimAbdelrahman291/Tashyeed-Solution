using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Constants;
using Tashyeed.Shared.Enums;
using Tashyeed.Web.ViewModels;
using Tashyeed.Web.ViewModels.SupervisorMDashboard;

namespace Tashyeed.Web.Controllers
{
    [Authorize(Roles = RoleNames.Supervisor)]
    public class SupervisorDashboardController : Controller
    {
        private readonly AppDBContext _context;

        public SupervisorDashboardController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var vm = new SupervisorDashboardVM
            {
                PendingCustodies = await _context.Custodies
                    .CountAsync(c => c.GivenToUserId == userId && c.Status == CustodyStatus.Pending),
                ConfirmedCustodies = await _context.Custodies
                    .CountAsync(c => c.GivenToUserId == userId && c.Status == CustodyStatus.Confirmed),
                TotalBalance = await _context.Custodies
                    .Where(c => c.GivenToUserId == userId && c.Status == CustodyStatus.Confirmed)
                    .SumAsync(c => c.RemainingAmount)
            };

            return View(vm);
        }
    }
}