using System.ComponentModel.DataAnnotations;
using Tashyeed.Shared.Enums;

namespace Tashyeed.Web.Modules.Procurement.ViewModels
{
    public class PurchaseOrderVM
    {
        [Required]
        public int PurchaseRequestId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Range(1, double.MaxValue, ErrorMessage = "المبلغ لازم يكون أكبر من 0")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "طريقة الدفع مطلوبة")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required(ErrorMessage = "الفاتورة مطلوبة")]
        public IFormFile InvoiceImage { get; set; } = null!;
    }
}
