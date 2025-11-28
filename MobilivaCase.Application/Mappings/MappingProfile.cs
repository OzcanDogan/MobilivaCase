using AutoMapper;
using MobilivaCase.Application.DTOs;
using MobilivaCase.Domain.Entities;

namespace MobilivaCase.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();

            CreateMap<ProductDetail, OrderDetail>();

            CreateMap<CreateOrderRequest, Order>()
                .ForMember(dest => dest.OrderDetails,
                           opt => opt.MapFrom(src => src.ProductDetails));
        }
    }
}
