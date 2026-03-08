using System;
using System.Collections.Generic;
using System.Text;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Shared.Enums;

namespace Tashyeed.Infrastructure.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int PurchaseRequestId { get; set; }
        public string PurchasedByUserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? InvoiceImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.FromCustody;
        // Navigation Properties
        public Project Project { get; set; } = null!;
        public PurchaseRequest PurchaseRequest { get; set; } = null!;
        public ApplicationUser PurchasedBy { get; set; } = null!;
    }
}
