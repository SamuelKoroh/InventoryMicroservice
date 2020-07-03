using InventoryService.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Domain.Services
{
    public interface IProductService
    {
        Task<bool> IsProductNameExisting(Product product);
        Task<Product> GetProductById(int id);
        Task<IEnumerable<Product>> GetProducts();
        Task<IEnumerable<Product>> GetProducts(bool isAvailable);
        Task<Product> CreateProduct(Product product);
        Task<Product> UpdateProduct(Product productToUpdate, Product product);
        Product DeleteProduct(Product product);
    }
}
