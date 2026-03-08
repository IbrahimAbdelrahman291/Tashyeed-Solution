using Tashyeed.Infrastructure.Entities;
using Tashyeed.Web.Modules.ProjectAssignment.ViewModels;

namespace Tashyeed.Web.Modules.ProjectAssignment.Services
{
    public interface IProjectAssignmentService
    {
        Task<IEnumerable<ProjectAssignmentListVM>> GetAllAsync();
        Task<IEnumerable<ProjectAssignmentListVM>> GetByProjectAsync(int projectId);
        Task<bool> AssignUserAsync(AssignUserVM vm);
        Task<bool> RemoveAssignmentAsync(int assignmentId);
    }
}
