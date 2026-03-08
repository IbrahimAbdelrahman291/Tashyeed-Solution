using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Interfaces;

namespace Tashyeed.Web.Modules.ProjectAssignment.Services
{
    public class ProjectAssignmentChecker : IProjectAssignmentChecker
    {
        private readonly AppDBContext _context;

        public ProjectAssignmentChecker(AppDBContext context)
        {
            _context = context;
        }

        public async Task<bool> HasAssignmentsAsync(string userId)
        {
            return await _context.ProjectAssignments.AnyAsync(pa => pa.UserId == userId);
        }
    }
}
