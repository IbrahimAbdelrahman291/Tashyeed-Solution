using AutoMapper;
using Tashyeed.Web.Modules.CustodyModule.ViewModels;
using Tashyeed.Infrastructure.Entities;
namespace Tashyeed.Web.Modules.CustodyModule.Mappings
{
    public class CustodyMappingProfile : AutoMapper.Profile
    {
        public CustodyMappingProfile()
        {
            CreateMap<Custody, CustodyListVM>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.GivenByName, opt => opt.MapFrom(src => src.GivenBy.FullName ?? src.GivenBy.Email))
                .ForMember(dest => dest.GivenToName, opt => opt.MapFrom(src => src.GivenTo.FullName ?? src.GivenTo.Email));

            CreateMap<AssignCustodyVM, Custody>();
        }
    }
}
