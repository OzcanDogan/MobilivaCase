using AutoMapper;
using MobilivaCase.DTOs;
using MobilivaCase.Models;

namespace MobilivaCase.Services
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
