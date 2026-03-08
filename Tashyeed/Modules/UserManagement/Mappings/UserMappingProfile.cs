using AutoMapper;
using Tashyeed.Infrastructure.Identity;
using Tashyeed.Web.Modules.UserManagement.ViewModels;

namespace Tashyeed.Web.Modules.UserManagement.Mappings
{
    public class UserMappingProfile : AutoMapper.Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, UserListVM>();
        }
    }
}
