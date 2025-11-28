using MobilivaCase.Domain.Entities;
using MobilivaCase.Application.DTOs;
namespace MobilivaCase.Application.Interfaces
{
    public interface IOrderService
    {
        public Task<int> CreateOrderAsync(CreateOrderRequest model);
    }
}
