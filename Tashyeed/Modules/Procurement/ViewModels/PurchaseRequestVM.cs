using System.ComponentModel.DataAnnotations;

namespace Tashyeed.Web.Modules.Procurement.ViewModels
{
    public class PurchaseRequestVM
    {
        [Required(ErrorMessage = "المشروع مطلوب")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "اسم الصنف مطلوب")]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية لازم تكون أكبر من 0")]
        public int Quantity { get; set; }

        public string? Description { get; set; }
    }
}
