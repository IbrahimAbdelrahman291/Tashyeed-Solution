using Tashyeed.Shared.Enums;

namespace Tashyeed.Web.Modules.Procurement.ViewModels
{
    public class ProcurementListVM
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string RequestedByName { get; set; } = string.Empty;
        public string RequestedById { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Order Details
        public decimal? OrderAmount { get; set; }
        public string? InvoiceImagePath { get; set; }
        public string? PurchasedByName { get; set; }
    }
}
