using System;
using System.Collections.Generic;
using System.Text;
using Tashyeed.Infrastructure.Identity;

namespace Tashyeed.Infrastructure.Entities
{
    public class MonthlyWorkerReport
    {
        public int Id { get; set; }
        public int WorkerId { get; set; }
        public string SubmittedByUserId { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal OvertimeHours { get; set; } = 0;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public Worker Worker { get; set; } = null!;
        public ApplicationUser SubmittedBy { get; set; } = null!;
    }
}
