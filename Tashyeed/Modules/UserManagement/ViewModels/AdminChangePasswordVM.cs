using System.ComponentModel.DataAnnotations;

namespace Tashyeed.Web.Modules.UserManagement.ViewModels
{
    public class AdminChangePasswordVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "الباسورد الجديد مطلوب")]
        [MinLength(6, ErrorMessage = "الباسورد لازم يكون 6 حروف على الأقل")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "تأكيد الباسورد مطلوب")]
        [Compare("NewPassword", ErrorMessage = "الباسورد مش متطابق")]
        public string ConfirmPassword { get; set; }
    }
}
