using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryService.Domain.Models
{
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }

        public Category()
        {
            Products = new Collection<Product>();
        }
    }
}