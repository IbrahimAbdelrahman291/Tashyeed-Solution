using System;
using System.Collections.Generic;
using System.Text;
using Tashyeed.Shared.Enums;

namespace Tashyeed.Infrastructure.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public decimal SpentAmount { get; set; } = 0;
        public ProjectStatus Status { get; set; } = ProjectStatus.Active; 
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ProjectAssignment> Assignments { get; set; } = [];
        public ICollection<Custody> Custodies { get; set; } = [];
        public ICollection<Expense> Expenses { get; set; } = [];
        public ICollection<PurchaseRequest> PurchaseRequests { get; set; } = [];
        public ICollection<WorkerRequest> WorkerRequests { get; set; } = [];
    }
}
