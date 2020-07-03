using InventoryService.Domain.Models;
using InventoryService.Domain.Repositories;

namespace InventoryService.Persistence.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(InventoryDbContext context) : base(context)
        {
        }

        public InventoryDbContext InventoryDbContext { get { return Context as InventoryDbContext; } }
    }
}
