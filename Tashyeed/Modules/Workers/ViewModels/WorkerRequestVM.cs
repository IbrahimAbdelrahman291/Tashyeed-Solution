using System.ComponentModel.DataAnnotations;
using Tashyeed.Shared.Enums;

namespace Tashyeed.Web.Modules.Workers.ViewModels
{
    public class WorkerRequestVM
    {
        [Required(ErrorMessage = "المشروع مطلوب")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "عدد العمال مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "العدد لازم يكون أكبر من 0")]
        public int NumberOfWorkers { get; set; }

        [Required(ErrorMessage = "نوع العمالة مطلوب")]
        public WorkerType WorkerType { get; set; }
    }
}
