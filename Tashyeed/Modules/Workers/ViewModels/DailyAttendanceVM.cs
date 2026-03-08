using System.ComponentModel.DataAnnotations;

namespace Tashyeed.Web.Modules.Workers.ViewModels
{
    public class DailyAttendanceVM
    {
        [Required]
        public int WorkerId { get; set; }

        public string WorkerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "التاريخ مطلوب")]
        public DateOnly AttendanceDate { get; set; }

        public bool IsPresent { get; set; }

        [Range(0, 24)]
        public decimal OvertimeHours { get; set; } = 0;
    }
}
