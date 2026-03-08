using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Enums;
using Tashyeed.Web.Modules.Accounting.ViewModels;

namespace Tashyeed.Web.Modules.Accounting.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDBContext _context;

        public ReportService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<ProjectReportVM> GetProjectReportAsync(int projectId, int month, int year)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project is null) return null;

            var custodiesTotal = await _context.Custodies
                .Where(c => c.ProjectId == projectId
                    && c.CreatedAt.Month == month
                    && c.CreatedAt.Year == year
                    && c.Status == CustodyStatus.Confirmed)
                .SumAsync(c => c.Amount);

            var expensesTotal = await _context.Expenses
                .Where(e => e.ProjectId == projectId
                    && e.CreatedAt.Month == month
                    && e.CreatedAt.Year == year
                    && e.Status == RequestStatus.Approved)
                .SumAsync(e => e.Amount);

            var procurementTotal = await _context.PurchaseOrders
                .Where(po => po.ProjectId == projectId
                    && po.CreatedAt.Month == month
                    && po.CreatedAt.Year == year)
                .SumAsync(po => po.Amount);

            var workers = await _context.Workers
                .Include(w => w.DailyAttendances)
                .Where(w => w.ProjectId == projectId)
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

            return new ProjectReportVM
            {
                ProjectName = project.Name,
                Budget = project.Budget,
                CustodiesTotal = custodiesTotal,
                ExpensesTotal = expensesTotal,
                ProcurementTotal = procurementTotal,
                WorkersTotal = workersTotal
            };
        }

        public async Task<FullReportVM> GetFullReportAsync(int month, int year)
        {
            var projects = await _context.Projects.ToListAsync();
            var result = new FullReportVM();

            foreach (var project in projects)
            {
                var projectReport = await GetProjectReportAsync(project.Id, month, year);
                if (projectReport is not null)
                    result.Projects.Add(projectReport);
            }

            return result;
        }
    }
}
