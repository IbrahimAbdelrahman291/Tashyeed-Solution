using System.ComponentModel.DataAnnotations;
using Tashyeed.Shared.Enums;

namespace Tashyeed.Web.Modules.Expenses.ViewModels
{
    public class AddExpenseVM
    {
        [Required(ErrorMessage = "المشروع مطلوب")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "نوع المصروف مطلوب")]
        public ExpenseType Type { get; set; }

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Range(1, double.MaxValue, ErrorMessage = "المبلغ لازم يكون أكبر من 0")]
        public decimal Amount { get; set; }

        public string? Notes { get; set; }

        [Required(ErrorMessage = "الفاتورة مطلوبة")]
        public IFormFile? InvoiceImage { get; set; }
    }
}
