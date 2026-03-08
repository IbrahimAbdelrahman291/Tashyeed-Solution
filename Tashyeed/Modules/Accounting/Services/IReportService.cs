using Tashyeed.Web.Modules.Accounting.ViewModels;

namespace Tashyeed.Web.Modules.Accounting.Services
{
    public interface IReportService
    {
        Task<ProjectReportVM> GetProjectReportAsync(int projectId, int month, int year);
        Task<FullReportVM> GetFullReportAsync(int month, int year);
    }
}
