using System;
using System.Collections.Generic;
using System.Text;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Shared.Enums;

namespace Tashyeed.Infrastructure.Entities
{
    public class WorkerRequest
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string RequestedByUserId { get; set; } = string.Empty;
        public string? ApprovedByUserId { get; set; }
        public int NumberOfWorkers { get; set; }
        public WorkerType WorkerType { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Project Project { get; set; } = null!;
        public ApplicationUser RequestedBy { get; set; } = null!;
        public ApplicationUser? ApprovedBy { get; set; }
    }
}
