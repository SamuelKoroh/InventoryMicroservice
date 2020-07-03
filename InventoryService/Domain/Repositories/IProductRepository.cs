using InventoryService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Domain.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryId(int categoryId);
        Task<IEnumerable<Product>> GetProductsByAvailability(bool isAvailable);
    }
}
