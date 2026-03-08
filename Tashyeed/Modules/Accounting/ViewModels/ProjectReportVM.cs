namespace Tashyeed.Web.Modules.Accounting.ViewModels
{
    public class ProjectReportVM
    {
        public string ProjectName { get; set; }
        public decimal Budget { get; set; }
        public decimal CustodiesTotal { get; set; }
        public decimal ExpensesTotal { get; set; }
        public decimal ProcurementTotal { get; set; }
        public decimal WorkersTotal { get; set; }
        public decimal GrandTotal => ExpensesTotal + ProcurementTotal + WorkersTotal;
        public decimal Remaining => Budget - GrandTotal;
    }
}
