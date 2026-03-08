using Tashyeed.Shared.Enums;

namespace Tashyeed.Web.Modules.Expenses.ViewModels
{
    public class ExpenseListVM
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string SubmittedByName { get; set; } = string.Empty;
        public string? ApprovedByName { get; set; }
        public ExpenseType Type { get; set; }
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
        public bool HasInvoice { get; set; }
        public string? InvoiceImagePath { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
