namespace Tashyeed.Web.ViewModels.AdminDashboard
{
    public class AdminDashboardVM
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int OnHoldProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public List<decimal> MonthlyCustodies { get; set; } = new();
        public List<decimal> MonthlyExpenses { get; set; } = new();
        public List<decimal> MonthlyProcurement { get; set; } = new();
        public List<decimal> MonthlyWorkers { get; set; } = new();
    }
}