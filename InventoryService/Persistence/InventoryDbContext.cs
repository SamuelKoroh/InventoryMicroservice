//using InventoryService.Bootstrap;
using InventoryService.Bootstrap;
using InventoryService.Domain.Models;
using InventoryService.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Persistence
{
    public class InventoryDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Request> Requests { get; set; }
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.SeedSampleData();
            base.OnModelCreating(modelBuilder);
        }
    }
}
