using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Entities;
using Tashyeed.Infrastructure.Persistence;

namespace Tashyeed.Web.Modules.Projects.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDBContext _context;

        public ProjectService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
            => await _context.Projects.ToListAsync();

        public async Task<Project?> GetByIdAsync(int id)
            => await _context.Projects.FindAsync(id);

        public async Task CreateAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var project = await GetByIdAsync(id);
            if (project is null) return;
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}
