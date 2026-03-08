using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tashyeed.Infrastructure.Entities;
using Tashyeed.Shared.Constants;
using Tashyeed.Web.Modules.Projects.Services;
using Tashyeed.Web.Modules.Projects.ViewModels;

namespace Tashyeed.Web.Modules.Projects.Controllers
{
    [Authorize(Roles =RoleNames.Admin)]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public ProjectsController(IProjectService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _projectService.GetAllAsync();
            var vm = _mapper.Map<IEnumerable<ProjectDetailsVM>>(projects);
            return View(vm);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var project = _mapper.Map<Project>(vm);
            await _projectService.CreateAsync(project);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project is null)
                return NotFound();

            var vm = _mapper.Map<EditProjectVM>(project);
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProjectVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var project = await _projectService.GetByIdAsync(vm.Id);
            if (project is null)
                return NotFound();

            _mapper.Map(vm, project);
            await _projectService.UpdateAsync(project);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project is null)
                return NotFound();

            var vm = _mapper.Map<ProjectDetailsVM>(project);
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _projectService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
