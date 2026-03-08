using AutoMapper;
using Tashyeed.Infrastructure.Entities;
using Tashyeed.Web.Modules.Procurement.ViewModels;

namespace Tashyeed.Web.Modules.Procurement.Mappings
{
    public class ProcurementMappingProfile : AutoMapper.Profile
    {
        public ProcurementMappingProfile()
        {
            CreateMap<PurchaseRequest, ProcurementListVM>()
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.RequestedById, opt => opt.MapFrom(src => src.RequestedByUserId))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(dest => dest.RequestedByName, opt => opt.MapFrom(src => src.RequestedBy.FullName ?? src.RequestedBy.Email))
                .ForMember(dest => dest.OrderAmount, opt => opt.MapFrom(src => src.PurchaseOrder != null ? src.PurchaseOrder.Amount : (decimal?)null))
                .ForMember(dest => dest.InvoiceImagePath, opt => opt.MapFrom(src => src.PurchaseOrder != null ? src.PurchaseOrder.InvoiceImagePath : null))
                .ForMember(dest => dest.PurchasedByName, opt => opt.MapFrom(src => src.PurchaseOrder != null ? src.PurchaseOrder.PurchasedBy.FullName ?? src.PurchaseOrder.PurchasedBy.Email : null));

            CreateMap<PurchaseRequestVM, PurchaseRequest>();

            CreateMap<PurchaseOrderVM, PurchaseOrder>()
                .ForMember(dest => dest.InvoiceImagePath, opt => opt.Ignore());
        }
    }
}
