using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Entities;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Enums;
using Tashyeed.Web.Modules.Workers.ViewModels;

namespace Tashyeed.Web.Modules.Workers.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;

        public WorkerService(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WorkerRequestListVM>> GetAllRequestsAsync()
        {
            var requests = await _context.WorkerRequests
                .Include(wr => wr.Project)
                .Include(wr => wr.RequestedBy)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WorkerRequestListVM>>(requests);
        }

        public async Task<IEnumerable<WorkerRequestListVM>> GetPendingRequestsAsync()
        {
            var requests = await _context.WorkerRequests
                .Include(wr => wr.Project)
                .Include(wr => wr.RequestedBy)
                .Where(wr => wr.Status == RequestStatus.Pending)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WorkerRequestListVM>>(requests);
        }

        public async Task CreateRequestAsync(WorkerRequestVM vm, string requestedByUserId)
        {
            var request = _mapper.Map<WorkerRequest>(vm);
            request.RequestedByUserId = requestedByUserId;
            _context.WorkerRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ApproveRequestAsync(int requestId, string approvedByUserId)
        {
            var request = await _context.WorkerRequests.FindAsync(requestId);
            if (request is null || request.Status != RequestStatus.Pending) return false;

            request.Status = RequestStatus.Approved;
            request.ApprovedByUserId = approvedByUserId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectRequestAsync(int requestId, string approvedByUserId)
        {
            var request = await _context.WorkerRequests.FindAsync(requestId);
            if (request is null || request.Status != RequestStatus.Pending) return false;

            request.Status = RequestStatus.Rejected;
            request.ApprovedByUserId = approvedByUserId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WorkerListVM>> GetWorkersByProjectAsync(int projectId)
        {
            var workers = await _context.Workers
                .Include(w => w.Project)
                .Where(w => w.ProjectId == projectId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WorkerListVM>>(workers);
        }

        public async Task AddWorkerAsync(AddWorkerVM vm, string supervisorUserId)
        {
            var worker = _mapper.Map<Worker>(vm);
            worker.SupervisorUserId = supervisorUserId;
            _context.Workers.Add(worker);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddDailyAttendanceAsync(DailyAttendanceVM vm, string submittedByUserId)
        {
            var exists = await _context.DailyAttendances
                .AnyAsync(da => da.WorkerId == vm.WorkerId && da.AttendanceDate == vm.AttendanceDate);

            if (exists) return false;

            var attendance = _mapper.Map<DailyAttendance>(vm);
            attendance.SubmittedByUserId = submittedByUserId;
            _context.DailyAttendances.Add(attendance);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MonthlyReportVM?> GetMonthlyReportAsync(int workerId, int month, int year)
        {
            var report = await _context.MonthlyWorkerReports
                .Include(mr => mr.Worker)
                .FirstOrDefaultAsync(mr => mr.WorkerId == workerId && mr.Month == month && mr.Year == year);

            if (report is null) return null;
            return _mapper.Map<MonthlyReportVM>(report);
        }

        public async Task SaveMonthlyReportAsync(MonthlyReportVM vm, string submittedByUserId)
        {
            var existing = await _context.MonthlyWorkerReports
                .FirstOrDefaultAsync(mr => mr.WorkerId == vm.WorkerId && mr.Month == vm.Month && mr.Year == vm.Year);

            if (existing is null)
            {
                var report = _mapper.Map<MonthlyWorkerReport>(vm);
                report.SubmittedByUserId = submittedByUserId;
                _context.MonthlyWorkerReports.Add(report);
            }
            else
            {
                existing.OvertimeHours = vm.OvertimeHours;
                existing.Notes = vm.Notes;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<WorkerRequestListVM>> GetMyRequestsAsync(string userId)
        {
            var requests = await _context.WorkerRequests
                .Include(wr => wr.Project)
                .Include(wr => wr.RequestedBy)
                .Where(wr => wr.RequestedByUserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WorkerRequestListVM>>(requests);
        }
        public async Task<WorkerPaymentSummaryVM> GetPaymentSummaryAsync(int workerId)
        {
            var worker = await _context.Workers
                .Include(w => w.Project)
                .FirstOrDefaultAsync(w => w.Id == workerId);

            if (worker is null) return new WorkerPaymentSummaryVM();

            var unpaidAttendances = await _context.DailyAttendances
                .Where(da => da.WorkerId == workerId && da.IsPresent && !da.IsPaid)
                .ToListAsync();

            return new WorkerPaymentSummaryVM
            {
                WorkerId = workerId,
                WorkerName = worker.Name,
                ProjectId = worker.ProjectId,
                DailyRate = worker.DailyRate,
                OvertimeHourRate = worker.OvertimeHourRate,
                DaysPresent = unpaidAttendances.Count,
                TotalOvertimeHours = unpaidAttendances.Sum(da => da.OvertimeHours)
            };
        }

        public async Task<bool> PayWorkerAsync(int workerId, string paidByUserId)
        {
            var worker = await _context.Workers.FindAsync(workerId);
            if (worker is null) return false;

            var unpaidAttendances = await _context.DailyAttendances
                .Where(da => da.WorkerId == workerId && da.IsPresent && !da.IsPaid)
                .ToListAsync();

            if (!unpaidAttendances.Any()) return false;

            // حساب الإجمالي
            var totalDays = unpaidAttendances.Count;
            var totalOvertimeHours = unpaidAttendances.Sum(da => da.OvertimeHours);
            var totalAmount = (totalDays * worker.DailyRate) + (totalOvertimeHours * worker.OvertimeHourRate);

            // خصم من عهدة المشرف
            var custody = await _context.Custodies
                .Where(c => c.GivenToUserId == paidByUserId
                    && c.ProjectId == worker.ProjectId
                    && c.Status == CustodyStatus.Confirmed)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();

            if (custody is null || custody.RemainingAmount < totalAmount) return false;

            custody.RemainingAmount -= totalAmount;

            // زيادة SpentAmount في المشروع
            var project = await _context.Projects.FindAsync(worker.ProjectId);
            if (project != null)
                project.SpentAmount += totalAmount;

            // تحديث الأيام لـ IsPaid = true
            foreach (var attendance in unpaidAttendances)
            {
                attendance.IsPaid = true;
                attendance.PaidAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task ToggleWorkerAsync(int workerId)
        {
            var worker = await _context.Workers.FindAsync(workerId);
            if (worker is null) return;

            worker.IsActive = !worker.IsActive;
            await _context.SaveChangesAsync();
        }
    }
}
