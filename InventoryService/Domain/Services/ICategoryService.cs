using InventoryService.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Domain.Services
{
    public interface ICategoryService
    {
        Task<bool> IsCategoryNameExisting(Category category);
        Task<Category> GetCategoryById(int id);
        Task<IEnumerable<Category>> GetCategories();
        Task<Category> CreateCategory(Category category);
        Task<Category> UpdateCategory(Category categoryToUpdate, Category category);
        Category DeleteCategory(Category category);
        Task<IEnumerable<Product>> GetProductsByCategoryId(int categoryId);
    }
}
