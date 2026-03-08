using System;
using System.Collections.Generic;
using System.Text;
using Tashyeed.Shared.Enums;

namespace Tashyeed.Infrastructure.Entities
{
    public class Worker
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string? SupervisorUserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public WorkerType Type { get; set; }
        public decimal DailyRate { get; set; }
        public decimal OvertimeHourRate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Project Project { get; set; } = null!;
        public ICollection<DailyAttendance> DailyAttendances { get; set; } = [];
        public ICollection<MonthlyWorkerReport> MonthlyReports { get; set; } = [];
    }
}
