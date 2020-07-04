namespace InventoryService.Domain.Models
{
    public class RequestApproval
    {
        public int RequestId { get; set; }
        public bool IsApproved { get; set; }
    }
}
