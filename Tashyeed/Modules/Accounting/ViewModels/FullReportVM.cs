namespace Tashyeed.Web.Modules.Accounting.ViewModels
{
    public class FullReportVM
    {
        public List<ProjectReportVM> Projects { get; set; } = new();
        public decimal GrandTotal => Projects.Sum(p => p.GrandTotal);
    }
}
