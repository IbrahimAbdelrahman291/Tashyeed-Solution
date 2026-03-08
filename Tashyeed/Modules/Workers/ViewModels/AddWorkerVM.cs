using System.ComponentModel.DataAnnotations;
using Tashyeed.Shared.Enums;

namespace Tashyeed.Web.Modules.Workers.ViewModels
{
    public class AddWorkerVM
    {
        [Required(ErrorMessage = "المشروع مطلوب")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "اسم العامل مطلوب")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "نوع العامل مطلوب")]
        public WorkerType Type { get; set; }

        [Required(ErrorMessage = "سعر اليومية مطلوب")]
        [Range(1, double.MaxValue, ErrorMessage = "السعر لازم يكون أكبر من 0")]
        public decimal DailyRate { get; set; }

        [Required(ErrorMessage = "سعر ساعة الأوفر تايم مطلوب")]
        [Range(0, double.MaxValue)]
        public decimal OvertimeHourRate { get; set; }
    }
}
