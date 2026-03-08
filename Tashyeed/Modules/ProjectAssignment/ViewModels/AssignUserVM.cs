using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tashyeed.Web.Modules.ProjectAssignment.ViewModels
{
    public class AssignUserVM
    {
        [Required(ErrorMessage = "المشروع مطلوب")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "الموظف مطلوب")]
        public string UserId { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
