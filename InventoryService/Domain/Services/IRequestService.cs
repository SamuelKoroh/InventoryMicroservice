using InventoryService.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Domain.Services
{
    public interface IRequestService
    {
        Task<IEnumerable<Request>> MyRequests(string requesterId);
        Task<Request> MakeRequest(Request request);
        Task<Request> UpdateRequest(Request requestToUpdate, bool isApproved);
        Task<bool> IsQuantityEnough(Request request);
        Task<bool> IsProductAvailable(Request request);
        Task<Request> GetRequestById(int requestId);
        Task<IEnumerable<Request>> GetAllRequest();
    }
}
