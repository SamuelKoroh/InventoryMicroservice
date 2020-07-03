using System;
using System.Threading.Tasks;
using InventoryService.Domain.Repositories;

namespace InventoryService.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        public IProductRepository Products { get; set; }
        public ICategoryRepository Categories { get; set; }
        public IRequestRepository Requests { get; set; }
        int CommitChanges();
        Task<int> CommitChangesAsync();
    }
}
