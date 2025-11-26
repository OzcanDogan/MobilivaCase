using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using MobilivaCase.Data;
using MobilivaCase.DTOs;
using MobilivaCase.Models;
using MobilivaCase.Services;
using System.Text.Json;

namespace MobilivaCase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        protected readonly AppDbContext _context;
        protected readonly RedisCacheService _redis;
        private readonly IMapper _mapper;
        public ProductController(AppDbContext context, RedisCacheService redis, IMapper mapper)
        {
            _context = context;
            _redis = redis;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ApiResponse<List<ProductDto>>> GetProducts([FromQuery] string? categoryName)
        {
            var res = new ApiResponse<List<ProductDto>>();

            try
            {
                var products = await _redis.Get<List<Product>>("products_cache");

                if (products == null)
                {
                    products = await _context.Products.ToListAsync();


                    await _redis.Set("products_cache", products,
                          new DistributedCacheEntryOptions
                          {
                              AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                          });
                }


                if (!string.IsNullOrEmpty(categoryName))
                {
                    products = products.Where(p => p.Category == categoryName).ToList();
                }

                var dto = _mapper.Map<List<ProductDto>>(products);
                res.Message = "Success";
                res.Data = dto;
                res.Status = ApiStatus.Success;
                return res;

            }
            catch (Exception ex)
            {
                res.Status = ApiStatus.Failed;
                res.Message ="Failed: " + ex.Message;
                return res;
            }
        }



    }
}
