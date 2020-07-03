using InventoryService.Domain;
using InventoryService.Domain.Repositories;
using InventoryService.Persistence.Repositories;
using System.Threading.Tasks;

namespace InventoryService.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InventoryDbContext _context;

        public IProductRepository Products { get ; set ; }
        public ICategoryRepository Categories { get ; set ; }
        public IRequestRepository Requests { get; set; }
        public UnitOfWork(InventoryDbContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
            Categories = new CategoryRepository(_context);
            Requests = new RequestRepository(_context);
        }
        public int CommitChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CommitChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
