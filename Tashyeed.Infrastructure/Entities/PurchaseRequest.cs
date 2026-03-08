using System;
using System.Collections.Generic;
using System.Text;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Shared.Enums;

namespace Tashyeed.Infrastructure.Entities
{
    public class PurchaseRequest
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string RequestedByUserId { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Project Project { get; set; } = null!;
        public ApplicationUser RequestedBy { get; set; } = null!;
        public PurchaseOrder? PurchaseOrder { get; set; }
    }
}
