using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MobilivaCase.Application.DTOs;
using MobilivaCase.Application.Interfaces;
using MobilivaCase.Application.Services;
using MobilivaCase.Data;


namespace MobilivaCase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;

        }
        [HttpGet]
        public async Task<ApiResponse<List<ProductDto>>> GetProducts([FromQuery] string? categoryName)
        {
            var res = new ApiResponse<List<ProductDto>>();

            try
            {
                var products = await _productService.GetProductsAsync(categoryName);
                res.Data = products;
                res.Status = ApiStatus.Success;
                res.Message = "Products retrieved successfully.";
                return res;
            }
            catch (Exception ex)
            {
                res.Status = ApiStatus.Failed;
                res.Message = "Failed: " + ex.Message;
                return res;
            }
        }



    }
}
