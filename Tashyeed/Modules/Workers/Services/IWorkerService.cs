using Tashyeed.Web.Modules.Workers.ViewModels;

namespace Tashyeed.Web.Modules.Workers.Services
{
    public interface IWorkerService
    {
        // Worker Requests
        Task<IEnumerable<WorkerRequestListVM>> GetAllRequestsAsync();
        Task<IEnumerable<WorkerRequestListVM>> GetPendingRequestsAsync();
        Task CreateRequestAsync(WorkerRequestVM vm, string requestedByUserId);
        Task<bool> ApproveRequestAsync(int requestId, string approvedByUserId);
        Task<bool> RejectRequestAsync(int requestId, string approvedByUserId);
        Task<IEnumerable<WorkerRequestListVM>> GetMyRequestsAsync(string userId);
        // Workers
        Task<IEnumerable<WorkerListVM>> GetWorkersByProjectAsync(int projectId);
        Task AddWorkerAsync(AddWorkerVM vm, string supervisorUserId);
        Task<WorkerPaymentSummaryVM> GetPaymentSummaryAsync(int workerId);
        Task<bool> PayWorkerAsync(int workerId, string paidByUserId);
        Task ToggleWorkerAsync(int workerId);

        // Daily Attendance
        Task<bool> AddDailyAttendanceAsync(DailyAttendanceVM vm, string submittedByUserId);
        // Monthly Report
        Task<MonthlyReportVM?> GetMonthlyReportAsync(int workerId, int month, int year);
        Task SaveMonthlyReportAsync(MonthlyReportVM vm, string submittedByUserId);
    }
}
