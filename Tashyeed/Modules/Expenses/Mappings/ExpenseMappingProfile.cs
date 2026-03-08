using AutoMapper;
using Tashyeed.Infrastructure.Entities;
using Tashyeed.Web.Modules.Expenses.ViewModels;

namespace Tashyeed.Web.Modules.Expenses.Mappings
{
    public class ExpenseMappingProfile : AutoMapper.Profile
    {
        public ExpenseMappingProfile()
        {
            CreateMap<Expense, ExpenseListVM>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.SubmittedByName, opt => opt.MapFrom(src => src.SubmittedBy.FullName ?? src.SubmittedBy.Email))
                .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedBy != null ? src.ApprovedBy.FullName ?? src.ApprovedBy.Email : null));

            CreateMap<AddExpenseVM, Expense>()
                .ForMember(dest => dest.InvoiceImagePath, opt => opt.Ignore());
        }
    }
}
