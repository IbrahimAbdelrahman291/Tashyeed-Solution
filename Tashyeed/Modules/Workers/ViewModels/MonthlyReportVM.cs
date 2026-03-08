using System.ComponentModel.DataAnnotations;

namespace Tashyeed.Web.Modules.Workers.ViewModels
{
    public class MonthlyReportVM
    {
        [Required]
        public int WorkerId { get; set; }

        public string WorkerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الشهر مطلوب")]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required(ErrorMessage = "السنة مطلوبة")]
        public int Year { get; set; }

        [Range(0, double.MaxValue)]
        public decimal OvertimeHours { get; set; } = 0;

        public string? Notes { get; set; }
    }
}
