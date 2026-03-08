using System.ComponentModel.DataAnnotations;

namespace Tashyeed.Web.Modules.CustodyModule.ViewModels
{
    public class AssignCustodyVM
    {
        [Required(ErrorMessage = "المشروع مطلوب")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "الموظف مطلوب")]
        public string GivenToUserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Range(1, double.MaxValue, ErrorMessage = "المبلغ لازم يكون أكبر من 0")]
        public decimal Amount { get; set; }
    }
}
