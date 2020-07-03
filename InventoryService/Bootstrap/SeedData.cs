//using InventoryService.Domain.Models;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace InventoryService.Bootstrap
//{
//    public static class SeedData
//    {
//        public static void SeedSampleData(this ModelBuilder builder)
//        {
//            builder.Entity<Category>().HasData(
//                new Category
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Electronics"
//                },
//                new Category
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Fashions"
//                });

//            builder.Entity<Product>().HasData(
//                new Product
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "LG DVD Player",
//                    CategoryId = 1,
//                    Quantity = 50,
//                    Price = 14000m,
//                    IsAvailable = true
//                },
//                new Product
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Sony DVD Player",
//                    CategoryId = 1,
//                    Quantity = 50,
//                    Price = 15000m,
//                    IsAvailable = true
//                },
//                new Product
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Samsung DVD Player",
//                    CategoryId = 1,
//                    Quantity = 50,
//                    Price = 15000m,
//                    IsAvailable = true
//                },
//                new Product
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Men Short",
//                    CategoryId = 2,
//                    Quantity = 50,
//                    Price = 3000m,
//                    IsAvailable = true
//                },
//                new Product
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Women sleeping garment",
//                    CategoryId = 2,
//                    Quantity = 50,
//                    Price = 5000m,
//                    IsAvailable = true
//                }
//                );
//        }
//    }
//}
