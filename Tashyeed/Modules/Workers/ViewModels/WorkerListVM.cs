using Tashyeed.Shared.Enums;

namespace Tashyeed.Web.Modules.Workers.ViewModels
{
    public class WorkerListVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public WorkerType Type { get; set; }
        public decimal DailyRate { get; set; }
        public decimal OvertimeHourRate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
