using Bogus; 
using Microsoft.EntityFrameworkCore;
using MobilivaCase.Models;

namespace MobilivaCase.Data
{
    public static class ProductSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            
            await context.Database.MigrateAsync();

            
            if (context.Products.Any())
                return;

            var faker = new Faker<Product>()
                .RuleFor(p => p.Description, f => f.Commerce.ProductName())
                .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
                .RuleFor(p => p.Unit, f => "Adet")
                .RuleFor(p => p.UnitPrice, f => f.Random.Decimal(10, 3000))
                .RuleFor(p => p.Status, f => true)
                .RuleFor(p => p.CreateDate, f => f.Date.Past())
                .RuleFor(p => p.UpdateDate, f => f.Date.Recent());

            var products = faker.Generate(1000);

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
