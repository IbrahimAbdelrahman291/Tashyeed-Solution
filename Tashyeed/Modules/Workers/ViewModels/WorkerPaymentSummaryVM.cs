namespace Tashyeed.Web.Modules.Workers.ViewModels
{
    public class WorkerPaymentSummaryVM
    {
        public int WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public decimal DailyRate { get; set; }
        public decimal OvertimeHourRate { get; set; }
        public int DaysPresent { get; set; }
        public decimal TotalOvertimeHours { get; set; }
        public decimal TotalDailyAmount => DaysPresent * DailyRate;
        public decimal TotalOvertimeAmount => TotalOvertimeHours * OvertimeHourRate;
        public decimal TotalAmount => TotalDailyAmount + TotalOvertimeAmount;
    }
}
