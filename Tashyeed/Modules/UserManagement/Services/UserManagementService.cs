using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Shared.Interfaces;
using Tashyeed.Web.Modules.UserManagement.ViewModels;

namespace Tashyeed.Web.Modules.UserManagement.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IProjectAssignmentChecker _assignmentChecker;

        public UserManagementService(UserManager<ApplicationUser> userManager, IMapper mapper, IProjectAssignmentChecker assignmentChecker)
        {
            _userManager = userManager;
            _mapper = mapper;
            _assignmentChecker = assignmentChecker;
        }

        public async Task<IEnumerable<UserListVM>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserListVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var vm = _mapper.Map<UserListVM>(user);
                vm.Role = roles.FirstOrDefault() ?? "";
                result.Add(vm);
            }

            return result;
        }

        public async Task<bool> CreateUserAsync(CreateUserVM vm)
        {
            var user = new ApplicationUser
            {
                FullName = vm.FullName,
                Email = vm.Email,
                UserName = vm.Email,
                Phone = vm.Phone,
                NationalId = vm.NationalId,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded) return false;

            await _userManager.AddToRoleAsync(user, vm.Role);
            return true;
        }

        public async Task<bool> ToggleUserStatusAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);
            return true;
        }
        public async Task<bool> DeleteUserAsync(string userId)
        {
            var hasAssignments = await _assignmentChecker.HasAssignmentsAsync(userId);
            if (hasAssignments) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            await _userManager.DeleteAsync(user);
            return true;
        }
        public async Task<bool> ChangeUserPasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }
        public async Task<UserListVM?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return null;
            return _mapper.Map<UserListVM>(user);
        }
    }
}
