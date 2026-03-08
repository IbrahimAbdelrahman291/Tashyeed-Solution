namespace Tashyeed.Web.Modules.ProjectAssignment.ViewModels
{
    public class ProjectAssignmentListVM
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }
}
