using AutoMapper;
using Tashyeed.Infrastructure.Entities;
using Tashyeed.Web.Modules.Projects.ViewModels;

namespace Tashyeed.Web.Modules.Projects.Mappings
{
    public class ProjectMappingProfile : AutoMapper.Profile
    {
        public ProjectMappingProfile()
        {
            // Entity => DetailsVM
            CreateMap<Project, ProjectDetailsVM>();

            // Entity => EditVM
            CreateMap<Project, EditProjectVM>();

            // CreateVM => Entity
            CreateMap<CreateProjectVM, Project>();

            // EditVM => Entity
            CreateMap<EditProjectVM, Project>();
        }
    }
}
