using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MobilivaCase.Data;
using MobilivaCase.Models;
using MobilivaCase.Services;

namespace MobilivaCase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {

        protected readonly AppDbContext _context;
        private readonly RabbitMqService _rabbit;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;


        public OrderController(AppDbContext context, RabbitMqService rabbit, IMapper mapper, ILogger<OrderController> logger)
        {
            _context = context;
            _rabbit = rabbit;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ApiResponse<int>> CreateOrder([FromBody] CreateOrderRequest model)
        {
            _logger.LogInformation("Sipariş isteği alındı: {@model}", model);
            var res = new ApiResponse<int>();
            var dto = _mapper.Map<Order>(model);

            try
            {


                var order = new Order
                {
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerGSM = model.CustomerGSM,
                    TotalAmount = model.ProductDetails.Sum(x => x.UnitPrice * x.Amount),
                    OrderDetails = model.ProductDetails.Select(p => new OrderDetail
                    {
                        ProductId = p.ProductId,
                        UnitPrice = p.UnitPrice,
                        Amount = p.Amount
                    }).ToList()
                };


                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation
                 (
                 "Order successfully created | Customer: {CustomerName}, Total: {TotalAmount}",
                 order.CustomerName,
                 order.TotalAmount
                 );




                // Mail göndermek için mesaj oluşturma işlemini burda yapıyorum.
                var mailMessage = new
                {
                    OrderId = order.Id,
                    CustomerEmail = order.CustomerEmail,
                    CustomerName = order.CustomerName,
                    Total = order.TotalAmount,
                    CreatedAt = DateTime.Now
                };
                // Mail mesajını RabbitMQ kuyruğuna gönderme işlemini de burda yapıyorum.
                _rabbit.Publish("SendMailQueue", mailMessage);

                res.Status = ApiStatus.Success;
                res.Message = "Order created successfully.";
                res.Data = order.Id;
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
