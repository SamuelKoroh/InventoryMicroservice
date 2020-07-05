using InventoryService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Bootstrap
{
    public static class SeedData
    {
        public static void SeedSampleData(this ModelBuilder builder)
        {
            builder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Electronics"
                },
                new Category
                {
                    Id = 2,
                    Name = "Fashion"
                });

            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "LG DVD Player",
                    CategoryId = 1,
                    Quantity = 50,
                    Price = 14000m,
                    IsAvailable = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Sony DVD Player",
                    CategoryId = 1,
                    Quantity = 50,
                    Price = 15000m,
                    IsAvailable = true
                },
                new Product
                {
                    Id = 3,
                    Name = "Samsung DVD Player",
                    CategoryId = 1,
                    Quantity = 50,
                    Price = 15000m,
                    IsAvailable = true
                },
                new Product
                {
                    Id = 4,
                    Name = "Men Short",
                    CategoryId = 2,
                    Quantity = 50,
                    Price = 3000m,
                    IsAvailable = true
                },
                new Product
                {
                    Id = 5,
                    Name = "Women sleeping garment",
                    CategoryId = 2,
                    Quantity = 50,
                    Price = 5000m,
                    IsAvailable = true
                });
        }
    }
}
