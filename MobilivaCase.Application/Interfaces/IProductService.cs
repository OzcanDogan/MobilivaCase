using MobilivaCase.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobilivaCase.Application.Interfaces
{
    public interface IProductService
    {
        public Task<List<ProductDto>> GetProductsAsync(string? categoryName);
    }
}
