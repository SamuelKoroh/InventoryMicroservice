using InventoryService.Domain.Models;
using InventoryService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryService.Persistence.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(InventoryDbContext context) : base(context)
        {
        }

        public InventoryDbContext InventoryDbContext { get { return Context as InventoryDbContext; } }

        public async Task<IEnumerable<Product>> GetProductsByAvailability(bool isAvailable)
        {
            return await InventoryDbContext.Products.Where(x => x.IsAvailable == isAvailable).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryId(int categoryId)
        {
            return await InventoryDbContext.Products.Where(x => x.CategoryId == categoryId).ToListAsync();
        }
    }
}
