using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MobilivaCase.Application.DTOs;
using MobilivaCase.Application.Interfaces;
using MobilivaCase.Data;

namespace MobilivaCase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;
        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }
        [HttpPost]
        public async Task<ApiResponse<int>> CreateOrder([FromBody] CreateOrderRequest model)
        {
            _logger.LogInformation("Sipariş isteği alındı: {@model}", model);
            var res = new ApiResponse<int>();
            try
            {
                var orderId = await _orderService.CreateOrderAsync(model);
                res.Status = ApiStatus.Success;
                res.Message = "Order created successfully.";
                res.Data = orderId;
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sipariş sırasında hata oldu!");
                res.Status = ApiStatus.Failed;
                res.Message = ex.Message;
                res.ErrorCode = "500";
                return res;
            }
        }
    }
}
