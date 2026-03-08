using Tashyeed.Web.Modules.Procurement.ViewModels;

namespace Tashyeed.Web.Modules.Procurement.Services
{
    public interface IProcurementService
    {
        Task<IEnumerable<ProcurementListVM>> GetAllAsync();
        Task<IEnumerable<ProcurementListVM>> GetByProjectAsync(int projectId);
        Task<IEnumerable<ProcurementListVM>> GetPendingAsync();
        Task CreateRequestAsync(PurchaseRequestVM vm, string requestedByUserId);
        Task<bool> CreateOrderAsync(PurchaseOrderVM vm, string purchasedByUserId);
        Task<bool> DeleteRequestAsync(int requestId, string userId);
        Task<bool> RevertOrderAsync(int requestId, string userId);
    }
}
