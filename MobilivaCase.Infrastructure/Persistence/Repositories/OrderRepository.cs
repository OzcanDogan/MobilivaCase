using Microsoft.EntityFrameworkCore;
using MobilivaCase.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobilivaCase.Infrastructure.Persistence.Repositories
{
    public class OrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateOrderAsync(Order model)
        {
            await _context.Orders.AddAsync(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }
    }
}
