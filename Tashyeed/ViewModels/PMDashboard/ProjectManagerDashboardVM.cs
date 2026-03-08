namespace Tashyeed.Web.ViewModels.PMDashboard
{
    public class ProjectManagerDashboardVM
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int OnHoldProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int TotalCustodiesSent { get; set; }
        public int PendingCustodies { get; set; }
    }
}
