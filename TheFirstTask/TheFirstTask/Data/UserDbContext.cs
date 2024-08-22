using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TheFirstTask.Data
{
    // Định nghĩa lớp UserDbContext kế thừa từ IdentityDbContext
    public class UserDbContext : IdentityDbContext<IdentityUser>
    {
        // Khởi tạo constructor nhận tham số DbContextOptions
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        // Phương thức OnModelCreating được gọi khi tạo model database
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Gọi phương thức OnModelCreating của lớp base
            base.OnModelCreating(builder);

            // Định nghĩa các hằng số cho ID của admin và role
            const string ADMIN_ID = "bda40279-03eb-4d5e-b2b3-543223c7ee64";
            const string ROLE_ID = "9a99d1dd-ecc5-4007-b9da-b4417c86e378";

            // Thêm dữ liệu mặc định cho vai trò "admin"
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = ROLE_ID,
                Name = "admin",
                NormalizedName = "admin"
            });

            // Tạo một instance của PasswordHasher để hash mật khẩu
            var hasher = new PasswordHasher<IdentityUser>();
            // Thêm dữ liệu mặc định cho user "admin"
            builder.Entity<IdentityUser>().HasData(new IdentityUser
            {
                Id = ADMIN_ID,
                UserName = "admin",
                NormalizedUserName = "admin",
                Email = "admin@gmail.com",
                NormalizedEmail = "admin@gmail.com",
                EmailConfirmed = false,
                PasswordHash = hasher.HashPassword(null, "Admin123#"), // Hash mật khẩu "Admin123#"
                SecurityStamp = string.Empty
            });

            // Thêm dữ liệu mặc định cho mối quan hệ giữa user "admin" và role "admin"
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = ROLE_ID,
                UserId = ADMIN_ID
            });
        }
    }
}