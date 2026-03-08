using System;
using System.Collections.Generic;
using System.Text;
using Tashyeed.Infrastructure.Identity;

namespace Tashyeed.Infrastructure.Entities
{
    public class DailyAttendance
    {
        public int Id { get; set; }
        public int WorkerId { get; set; }
        public string SubmittedByUserId { get; set; } = string.Empty;
        public DateOnly AttendanceDate { get; set; }
        public bool IsPresent { get; set; }
        public decimal OvertimeHours { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPaid { get; set; } = false;
        public DateTime? PaidAt { get; set; }

        // Navigation Properties
        public Worker Worker { get; set; } = null!;
        public ApplicationUser SubmittedBy { get; set; } = null!;
    }
}
