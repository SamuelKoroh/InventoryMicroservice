using InventoryService.Domain;
using InventoryService.Domain.Models;
using InventoryService.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Persistence.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Category> CreateCategory(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CommitChangesAsync();

            return category;
        }

        public Category DeleteCategory(Category category)
        {
            _unitOfWork.Categories.Remove(category);
            _unitOfWork.CommitChanges();

            return category;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _unitOfWork.Categories.GetAllAsync();
        }

        public async Task<Category> GetCategoryById(int id)
        {
            return await _unitOfWork.Categories.GetByIdAsync(id);
        }

        public async Task<bool> IsCategoryNameExisting(Category category)
        {
            return await _unitOfWork.Categories.IsExistingAsync(x => 
                x.Name.ToLower().Trim() == category.Name.ToLower().Trim());
        }

        public async Task<Category> UpdateCategory(Category categoryToUpdate, Category category)
        {
            categoryToUpdate.Name = category.Name;

            await _unitOfWork.CommitChangesAsync();

            return categoryToUpdate;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryId(int categoryId)
        {
            return await _unitOfWork.Products.GetProductsByCategoryId(categoryId);
        }
    }
}
