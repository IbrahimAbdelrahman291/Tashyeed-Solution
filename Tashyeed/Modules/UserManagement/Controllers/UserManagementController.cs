using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tashyeed.Shared.Constants;
using Tashyeed.Web.Modules.UserManagement.Services;
using Tashyeed.Web.Modules.UserManagement.ViewModels;

namespace Tashyeed.Web.Modules.UserManagement.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class UserManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IMapper _mapper;

        public UserManagementController(IUserManagementService userManagementService, IMapper mapper)
        {
            _userManagementService = userManagementService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result = await _userManagementService.CreateUserAsync(vm);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "حصل خطأ أثناء إنشاء المستخدم");
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string userId)
        {
            await _userManagementService.ToggleUserStatusAsync(userId);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId)
        {
            var result = await _userManagementService.DeleteUserAsync(userId);
            if (!result)
                TempData["Error"] = "مش هينفع تمسح الموظف ده لأنه متعين على مشروع";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ChangePassword(string userId)
        {
            var user = await _userManagementService.GetUserByIdAsync(userId);
            if (user is null) return NotFound();

            var vm = new AdminChangePasswordVM
            {
                UserId = userId,
                UserName = user.FullName ?? user.Email
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(AdminChangePasswordVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result = await _userManagementService.ChangeUserPasswordAsync(vm.UserId, vm.NewPassword);
            if (!result)
            {
                TempData["Error"] = "حصل خطأ أثناء تغيير الباسورد";
                return View(vm);
            }

            TempData["Success"] = "تم تغيير الباسورد بنجاح ✓";
            return RedirectToAction(nameof(Index));
        }
    }
}
