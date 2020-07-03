using InventoryService.Domain;
using InventoryService.Domain.Models;
using InventoryService.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Persistence.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Product> CreateProduct(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CommitChangesAsync();

            return product;
        }

        public Product DeleteProduct(Product product)
        {
             _unitOfWork.Products.Remove(product);
             _unitOfWork.CommitChanges();

            return product;
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _unitOfWork.Products.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts(bool isAvailable)
        {
            return await _unitOfWork.Products.GetProductsByAvailability(isAvailable);
        }

        public async Task<bool> IsProductNameExisting(Product product)
        {
            return await _unitOfWork.Products.IsExistingAsync(x =>
                x.Name.ToLower().Trim() == product.Name.ToLower().Trim());
        }

        public async Task<Product> UpdateProduct(Product productToUpdate, Product product)
        {
            productToUpdate.Name = product.Name;
            productToUpdate.Quantity = product.Quantity;
            productToUpdate.Price = product.Price;
            productToUpdate.CategoryId = product.CategoryId;
            productToUpdate.IsAvailable = product.IsAvailable;

            await _unitOfWork.CommitChangesAsync();

            return productToUpdate;
        }
    }
}
