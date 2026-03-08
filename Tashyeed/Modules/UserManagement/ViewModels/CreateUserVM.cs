using System.ComponentModel.DataAnnotations;

namespace Tashyeed.Web.Modules.UserManagement.ViewModels
{
    public class CreateUserVM
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور لازم تكون 6 أحرف على الأقل")]
        public string Password { get; set; } = string.Empty;

        public string? NationalId { get; set; }
        public string? Phone { get; set; }

        [Required(ErrorMessage = "الدور مطلوب")]
        public string Role { get; set; } = string.Empty;
    }
}
