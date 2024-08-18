using Microsoft.EntityFrameworkCore;
using TheFirstTask.Model;

namespace TheFirstTask.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<TaskOrder> TaskOrders { get; set; }
        public DbSet<TaskOrderDetail> TaskOrderDetails { get; set; }
    }
}
