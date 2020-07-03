using InventoryService.Domain;
using InventoryService.Domain.Models;
using InventoryService.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Persistence.Services
{
    public class RequestService : IRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Request> GetRequestById(int requestId)
        {
            return await _unitOfWork.Requests.GetByIdAsync(requestId);
        }
        public async Task<Request> MakeRequest(Request request)
        {
            await _unitOfWork.Requests.AddAsync(request);
            await _unitOfWork.CommitChangesAsync();

            return request;
        }

        public async Task<bool> IsQuantityEnough(Request request)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);

            return product.Quantity >= request.Quantity;
        }

        public async Task<bool> IsProductAvailable(Request request)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);

            return product.IsAvailable;
        }
        public async Task<IEnumerable<Request>> MyRequests(string requesterId)
        {
            return await _unitOfWork.Requests.GetRequests(requesterId);
        }

        public async Task<Request> UpdateRequest(Request requestToUpdate, bool isApproved)
        {
            requestToUpdate.Status = isApproved ? "Approved" : "Rejected";
            requestToUpdate.IsApproved = isApproved;

            await _unitOfWork.CommitChangesAsync();

            return requestToUpdate;
        }
    }
}
