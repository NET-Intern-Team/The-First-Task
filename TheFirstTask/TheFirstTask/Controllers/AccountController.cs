using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TheFirstTask.Model;

namespace TheFirstTask.Controllers
{
    [Route("api/[controller]")] // Định nghĩa route cho controller này
    [ApiController] // Xác định đây là một controller API
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager; // UserManager dùng để quản lý người dùng
        private readonly RoleManager<IdentityRole> _roleManager; // RoleManager dùng để quản lý vai trò
        private readonly IConfiguration _configuration; // IConfiguration dùng để truy cập các setting cấu hình

        // Khởi tạo các service trong constructor
        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // Endpoint cho đăng ký tài khoản mới
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            // Tạo một user mới từ model
            var user = new IdentityUser { UserName = model.Username };
            // Thực hiện đăng ký user
            var result = await _userManager.CreateAsync(user, model.Password);
            // Nếu đăng ký thành công
            if (result.Succeeded)
            {
                return Ok(new { message = "User register successfully" }); // Trả về thông báo thành công
            }
            // Nếu đăng ký thất bại
            return BadRequest(); // Trả về lỗi
        }

        // Endpoint cho đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            // Tìm user dựa vào username
            var user = await _userManager.FindByNameAsync(model.Username);
            // Kiểm tra user tồn tại và mật khẩu đúng
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Lấy danh sách vai trò của user
                var userRoles = await _userManager.GetRolesAsync(user);
                // Khởi tạo danh sách các Claim (thông tin về user)
                var authClaims = new List<Claim>
                 {
                     new Claim(JwtRegisteredClaimNames.Sub, user.UserName!), // Thêm claim cho username
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Thêm claim cho ID token
                 };

                // Thêm các claim cho vai trò
                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                // Tạo token JWT
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"], // Issuer của token
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)), // Thời hạn của token
                    claims: authClaims, // Danh sách các claim
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)), // Signing credential (dùng để ký token)
                    SecurityAlgorithms.HmacSha256 // Algorithm để ký token
                    )
                    );
                // Trả về token
                return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            // Nếu đăng nhập thất bại
            return Unauthorized(); // Trả về lỗi unauthorized
        }

        // Endpoint để thêm vai trò mới
        [HttpPost("add-role")]
        [Authorize(Roles = "admin")] // Chỉ cho phép admin thêm vai trò
        public async Task<IActionResult> AddRole([FromBody] string role)
        {
            // Kiểm tra vai trò đã tồn tại chưa
            if (!await _roleManager.RoleExistsAsync(role))
            {
                // Thực hiện tạo vai trò mới
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                // Nếu tạo thành công
                if (result.Succeeded)
                {
                    return Ok(new { message = "Role added successfully" }); // Trả về thông báo thành công
                }
                // Nếu tạo thất bại
                return BadRequest(result.Errors); // Trả về lỗi
            }
            // Nếu vai trò đã tồn tại
            return BadRequest("Role already exists"); // Trả về lỗi
        }

        // Endpoint để gán vai trò cho user
        [HttpPost("assign-role")]
        [Authorize(Roles = "admin")] // Chỉ cho phép admin gán vai trò
        public async Task<IActionResult> AssignRole([FromBody] UserRole model)
        {
            // Tìm user dựa vào username
            var user = await _userManager.FindByNameAsync(model.Username);
            // Kiểm tra user tồn tại
            if (user == null)
            {
                return BadRequest("User not found"); // Trả về lỗi
            }

            // Gán vai trò cho user
            var result = await _userManager.AddToRoleAsync(user, model.Role);

            // Nếu gán thành công
            if (result.Succeeded)
            {
                return Ok(new { message = "Role assigned successfully" }); // Trả về thông báo thành công
            }
            // Nếu gán thất bại
            return BadRequest(result.Errors); // Trả về lỗi
        }
    }
}