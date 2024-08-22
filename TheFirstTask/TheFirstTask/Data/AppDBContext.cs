using Microsoft.EntityFrameworkCore;
using TheFirstTask.Model;

namespace TheFirstTask.Data
{
    // Định nghĩa lớp AppDBContext kế thừa từ DbContext
    public class AppDBContext : DbContext
    {
        // Khởi tạo constructor nhận tham số DbContextOptions
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        // Khai báo các DbSet cho mỗi bảng trong database
        public DbSet<Category> Categories { get; set; } // DbSet cho bảng Category
        public DbSet<Product> Products { get; set; } // DbSet cho bảng Product
        public DbSet<Order> Orders { get; set; } // DbSet cho bảng Order
        public DbSet<TaskOrder> TaskOrders { get; set; } // DbSet cho bảng TaskOrder
        public DbSet<TaskOrderDetail> TaskOrderDetails { get; set; } // DbSet cho bảng TaskOrderDetail
    }
}