using InventoryService.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Domain.Repositories
{
    public interface IRequestRepository : IRepository<Request>
    {
        Task<IEnumerable<Request>> GetRequests(string requesterId);
    }
}
