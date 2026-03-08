using Tashyeed.Shared.Enums;

namespace Tashyeed.Web.Modules.CustodyModule.ViewModels
{
    public class CustodyListVM
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string GivenByName { get; set; } = string.Empty;
        public string GivenToName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal RemainingAmount { get; set; }
        public CustodyStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
    }
}
