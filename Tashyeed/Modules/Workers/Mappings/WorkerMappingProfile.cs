using AutoMapper;
using Tashyeed.Infrastructure.Entities;
using Tashyeed.Web.Modules.Workers.ViewModels;

namespace Tashyeed.Web.Modules.Workers.Mappings
{
    public class WorkerMappingProfile : AutoMapper.Profile
    {
        public WorkerMappingProfile()
        {
            CreateMap<Worker, WorkerListVM>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name));

            CreateMap<AddWorkerVM, Worker>();

            CreateMap<WorkerRequest, WorkerRequestListVM>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.RequestedByName, opt => opt.MapFrom(src => src.RequestedBy.FullName ?? src.RequestedBy.Email));
            CreateMap<WorkerRequestVM, WorkerRequest>();

            CreateMap<DailyAttendanceVM, DailyAttendance>();

            CreateMap<MonthlyReportVM, MonthlyWorkerReport>();
        }
    }
}
