using System.ComponentModel;

namespace InventoryService.Domain.Models
{
    public class Request
    {
        public int Id { get; set; }
        public string Requester { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        [DefaultValue(false)]
        public bool IsApproved { get; set; }
        [DefaultValue("pending")]
        public string Status { get; set; }
    }
}
