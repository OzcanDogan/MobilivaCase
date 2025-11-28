using AutoMapper;
using MobilivaCase.Application.DTOs;
using MobilivaCase.Application.Interfaces;
using MobilivaCase.Domain.Entities;
using MobilivaCase.Infrastructure.MessageQueue;
using MobilivaCase.Infrastructure.Persistence.Repositories;

namespace MobilivaCase.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly RabbitMqService _rabbitMqService;
        public OrderService(OrderRepository orderRepository, IMapper mapper, RabbitMqService rabbitMqService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _rabbitMqService = rabbitMqService;
        }
        public async Task<int> CreateOrderAsync(CreateOrderRequest model)
        {
            model.TotalAmount = model.ProductDetails.Sum(x => x.UnitPrice * x.Amount);
            var dto = _mapper.Map<Order>(model);
            var orderId = await _orderRepository.CreateOrderAsync(dto);
            // Mail göndermek için mesaj oluşturma işlemini burda yapıyorum.
            var mailMessage = new
            {
                OrderId = orderId,
                model.CustomerEmail,
                model.CustomerName,
                Total = model.TotalAmount,
                CreatedAt = DateTime.Now
            };
            // Mail mesajını RabbitMQ kuyruğuna gönderme işlemini de burda yapıyorum.
            _rabbitMqService.Publish("SendMailQueue", mailMessage);

            return orderId;

        }
    }
}
