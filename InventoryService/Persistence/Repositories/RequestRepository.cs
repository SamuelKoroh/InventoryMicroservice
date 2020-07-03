using InventoryService.Domain.Models;
using InventoryService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryService.Persistence.Repositories
{
    public class RequestRepository : Repository<Request>, IRequestRepository
    {
        public RequestRepository(InventoryDbContext context) : base(context)
        {
        }

        public InventoryDbContext InventoryDbContext { get { return Context as InventoryDbContext; } }

        public async Task<IEnumerable<Request>> GetRequests(string requesterId)
        {
            return await InventoryDbContext.Requests.Where(x => x.Requester == requesterId)
                .OrderByDescending(x => x.Id).ToListAsync();
        }
    }


}
