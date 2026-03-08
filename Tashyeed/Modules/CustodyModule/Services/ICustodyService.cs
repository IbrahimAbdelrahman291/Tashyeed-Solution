using Tashyeed.Web.Modules.CustodyModule.ViewModels;

namespace Tashyeed.Web.Modules.CustodyModule.Services
{
    public interface ICustodyService
    {
        Task<IEnumerable<CustodyListVM>> GetAllAsync();
        Task<IEnumerable<CustodyListVM>> GetByProjectAsync(int projectId);
        Task<IEnumerable<CustodyListVM>> GetMyPendingCustodiesAsync(string userId);
        Task AssignCustodyAsync(AssignCustodyVM vm, string givenByUserId);
        Task<bool> ConfirmCustodyAsync(int custodyId, string userId);
        Task<bool> RejectCustodyAsync(int custodyId, string userId);
    }
}
