using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Web.Modules.ProjectAssignment.ViewModels;

namespace Tashyeed.Web.Modules.ProjectAssignment.Services
{
    public class ProjectAssignmentService : IProjectAssignmentService
    {
        private readonly AppDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectAssignmentService(AppDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<ProjectAssignmentListVM>> GetAllAsync()
        {
            return await _context.ProjectAssignments
                .Include(pa => pa.Project)
                .Include(pa => pa.User)
                .Select(pa => new ProjectAssignmentListVM
                {
                    Id = pa.Id,
                    ProjectName = pa.Project.Name,
                    UserName = pa.User.FullName ?? pa.User.Email!,
                    Role = pa.Role,
                    AssignedAt = pa.AssignedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectAssignmentListVM>> GetByProjectAsync(int projectId)
        {
            return await _context.ProjectAssignments
                .Include(pa => pa.Project)
                .Include(pa => pa.User)
                .Where(pa => pa.ProjectId == projectId)
                .Select(pa => new ProjectAssignmentListVM
                {
                    Id = pa.Id,
                    ProjectName = pa.Project.Name,
                    UserName = pa.User.FullName ?? pa.User.Email!,
                    Role = pa.Role,
                    AssignedAt = pa.AssignedAt
                })
                .ToListAsync();
        }

        public async Task<bool> AssignUserAsync(AssignUserVM vm)
        {
            // نتأكد إن الموظف مش متعين على نفس المشروع
            var exists = await _context.ProjectAssignments
                .AnyAsync(pa => pa.ProjectId == vm.ProjectId && pa.UserId == vm.UserId);
            if (exists) return false;

            // نتأكد إن الـ Role بتاع التعيين = الـ Identity Role بتاع الشخص
            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user is null) return false;

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains(vm.Role)) return false;

            var assignment = new Tashyeed.Infrastructure.Entities.ProjectAssignment
            {
                ProjectId = vm.ProjectId,
                UserId = vm.UserId,
                Role = vm.Role
            };

            _context.ProjectAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAssignmentAsync(int assignmentId)
        {
            var assignment = await _context.ProjectAssignments.FindAsync(assignmentId);
            if (assignment is null) return false;

            _context.ProjectAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
