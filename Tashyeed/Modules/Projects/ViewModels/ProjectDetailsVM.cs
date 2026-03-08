using Tashyeed.Shared.Enums;

namespace Tashyeed.Web.Modules.Projects.ViewModels
{
    public class ProjectDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingBudget => Budget - SpentAmount;
        public ProjectStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
