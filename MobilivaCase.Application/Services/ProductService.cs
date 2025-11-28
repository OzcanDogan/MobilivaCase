using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using MobilivaCase.Application.DTOs;
using MobilivaCase.Application.Interfaces;
using MobilivaCase.Domain.Entities;
using MobilivaCase.Infrastructure.Cache;
using MobilivaCase.Infrastructure.Persistence.Repositories;


namespace MobilivaCase.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly RedisCacheService _redis;
        private readonly IMapper _mapper;
        public ProductService(ProductRepository productRepository, RedisCacheService redisCacheService, IMapper mapper)
        {
            _productRepository = productRepository;
            _redis = redisCacheService;
            _mapper = mapper;
        }
        public async Task<List<ProductDto>> GetProductsAsync(string? categoryName)
        {
            var products = await _redis.Get<List<Product>>("products_cache");

            if (products == null)
            {
                products = await _productRepository.GetProductsAsync();


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
            return dto;
        }
    }
}
