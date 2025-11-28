using Microsoft.EntityFrameworkCore;
using MobilivaCase.Domain.Entities;

namespace MobilivaCase.Infrastructure.Persistence.Repositories
{
    public class ProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            return products;
        }
    }
}
