using System;
using System.Collections.Generic;
using System.Text;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Shared.Enums;

namespace Tashyeed.Infrastructure.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string SubmittedByUserId { get; set; } = string.Empty;
        public string? ApprovedByUserId { get; set; }
        public ExpenseType Type { get; set; }
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
        public bool HasInvoice { get; set; }
        public string? InvoiceImagePath { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Project Project { get; set; } = null!;
        public ApplicationUser SubmittedBy { get; set; } = null!;
        public ApplicationUser? ApprovedBy { get; set; }
    }
}
