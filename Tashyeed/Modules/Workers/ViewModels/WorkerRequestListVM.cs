using Tashyeed.Shared.Enums;

namespace Tashyeed.Web.Modules.Workers.ViewModels
{
    public class WorkerRequestListVM
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string RequestedByName { get; set; } = string.Empty;
        public int NumberOfWorkers { get; set; }
        public WorkerType WorkerType { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
