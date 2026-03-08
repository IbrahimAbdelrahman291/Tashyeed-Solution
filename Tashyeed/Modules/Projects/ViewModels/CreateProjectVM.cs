using System.ComponentModel.DataAnnotations;

namespace Tashyeed.Web.Modules.Projects.ViewModels
{
    public class CreateProjectVM
    {
        [Required(ErrorMessage = "اسم المشروع مطلوب")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "الموقع مطلوب")]
        [MaxLength(300)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "الميزانية مطلوبة")]
        [Range(1, double.MaxValue, ErrorMessage = "الميزانية لازم تكون أكبر من 0")]
        public decimal Budget { get; set; }

        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
