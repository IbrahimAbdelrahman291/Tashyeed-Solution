using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Constants;
using Tashyeed.Shared.Enums;
using Tashyeed.Web.ViewModels.PMDashboard;

namespace Tashyeed.Web.Controllers
{
    [Authorize(Roles = RoleNames.ProjectManager)]
    public class ProjectManagerDashboardController : Controller
    {
        private readonly AppDBContext _context;

        public ProjectManagerDashboardController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var myProjectIds = await _context.ProjectAssignments
                .Where(pa => pa.UserId == userId && pa.Role == RoleNames.ProjectManager)
                .Select(pa => pa.ProjectId)
                .ToListAsync();

            var vm = new ProjectManagerDashboardVM
            {
                TotalProjects = myProjectIds.Count,
                ActiveProjects = await _context.Projects
                    .CountAsync(p => myProjectIds.Contains(p.Id) && p.Status == ProjectStatus.Active),
                OnHoldProjects = await _context.Projects
                    .CountAsync(p => myProjectIds.Contains(p.Id) && p.Status == ProjectStatus.OnHold),
                CompletedProjects = await _context.Projects
                    .CountAsync(p => myProjectIds.Contains(p.Id) && p.Status == ProjectStatus.Completed),
                TotalCustodiesSent = await _context.Custodies
                    .CountAsync(c => c.GivenByUserId == userId),
                PendingCustodies = await _context.Custodies
                    .CountAsync(c => c.GivenByUserId == userId && c.Status == CustodyStatus.Pending)
            };

            return View(vm);
        }
    }
}
