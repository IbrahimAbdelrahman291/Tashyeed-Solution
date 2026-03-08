using Tashyeed.Web.Modules.UserManagement.ViewModels;

namespace Tashyeed.Web.Modules.UserManagement.Services
{
    public interface IUserManagementService
    {
        Task<IEnumerable<UserListVM>> GetAllUsersAsync();
        Task<bool> CreateUserAsync(CreateUserVM vm);
        Task<bool> ToggleUserStatusAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> ChangeUserPasswordAsync(string userId, string newPassword);
        Task<UserListVM?> GetUserByIdAsync(string userId);
    }
}
