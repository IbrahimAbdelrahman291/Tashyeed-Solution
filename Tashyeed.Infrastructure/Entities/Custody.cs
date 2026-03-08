using System;
using System.Collections.Generic;
using System.Text;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Shared.Enums;

namespace Tashyeed.Infrastructure.Entities
{
    public class Custody
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string GivenByUserId { get; set; } = string.Empty;   // مدير المشروع
        public string GivenToUserId { get; set; } = string.Empty;   // مدير المشتريات أو المشرف
        public decimal Amount { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public CustodyStatus Status { get; set; } = CustodyStatus.Pending;
        public DateTime? ConfirmedAt { get; set; }

        // Navigation Properties
        public Project Project { get; set; } = null!;
        public ApplicationUser GivenBy { get; set; } = null!;
        public ApplicationUser GivenTo { get; set; } = null!;
    }
}
