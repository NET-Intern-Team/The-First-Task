using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheFirstTask.Data;
using TheFirstTask.Model;

namespace TheFirstTask.Controllers
{
    // Định nghĩa route cho controller
    [Route("api/Product")]
    [ApiController] // Xác định đây là một controller API
    public class ProductController : ControllerBase
    {
        private readonly AppDBContext _dbContext; // Biến lưu trữ context của database

        // Khởi tạo context trong constructor
        public ProductController(AppDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        // Endpoint để lấy danh sách các products
        [HttpGet(Name = "GetProducts")] // Xác định đây là method GET
        [ProducesResponseType(StatusCodes.Status200OK)] // Xác định loại phản hồi thành công (200 OK)
        public async Task<ActionResult<ICollection<Product>>> GetProducts(int page = 1, int pageSize = 10)
        {
            // Lấy tổng số products
            var totalCount = await _dbContext.Products.CountAsync();
            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            // Lấy danh sách products trên trang hiện tại
            var productsPerPage = await _dbContext.Products.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            // Trả về danh sách products
            return Ok(productsPerPage);
        }

        // Endpoint để lấy thông tin chi tiết của một product
        [HttpGet("{id:int}", Name = "GetProduct")] // Xác định đây là method GET, id là một số nguyên
        [ProducesResponseType(StatusCodes.Status200OK)] // Xác định loại phản hồi thành công (200 OK)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Xác định loại phản hồi không tìm thấy (404 NotFound)
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            // Kiểm tra id có hợp lệ không
            if (id == 0)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm product theo id
            var pro = await _dbContext.Products.FirstOrDefaultAsync(s => s.Id == id);
            // Kiểm tra product có tồn tại không
            if (pro == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Trả về thông tin product
            return Ok(pro);
        }

        // Endpoint để tạo một product mới
        [HttpPost(Name = "CreateProduct")] // Xác định đây là method POST
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status201Created)] // Xác định loại phản hồi tạo thành công (201 Created)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Xác định loại phản hồi lỗi server (500 InternalServerError)
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            // Kiểm tra tên product đã tồn tại chưa
            if (_dbContext.Products.FirstOrDefault(s => s.Name.ToLower() == product.Name.ToLower()) != null)
            {
                // Thêm lỗi vào ModelState
                ModelState.AddModelError("ErrorNameDuplicate", "Product name already existed");
                // Trả về lỗi 400 BadRequest
                return BadRequest(ModelState);
            }
            // Kiểm tra product và category có hợp lệ không
            if (product == null || _dbContext.Categories.FirstOrDefault(s => s.Id == product.CategoryId) == null)
            {
                return BadRequest(product); // Trả về lỗi 400 BadRequest
            }
            // Kiểm tra id có hợp lệ không
            if (product.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError); // Trả về lỗi 500 InternalServerError
            }
            // Thêm product vào database
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
            // Trả về thông tin product mới tạo
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        // Endpoint để xóa một product
        [HttpDelete("{id:int}", Name = "DeleteProduct")] // Xác định đây là method DELETE, id là một số nguyên
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Xác định loại phản hồi xóa thành công (204 NoContent)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Xác định loại phản hồi không tìm thấy (404 NotFound)
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Kiểm tra id có hợp lệ không
            if (id == 0)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm product theo id
            var pro = await _dbContext.Products.FirstOrDefaultAsync(s => s.Id == id);
            // Kiểm tra product có tồn tại không
            if (pro == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Xóa product khỏi database
            _dbContext.Products.Remove(pro);
            _dbContext.SaveChanges();
            // Trả về phản hồi xóa thành công (204 NoContent)
            return NoContent();
        }

        // Endpoint để cập nhật thông tin của một product
        [HttpPut("{id:int}", Name = "UpdateProduct")] // Xác định đây là method PUT, id là một số nguyên
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Xác định loại phản hồi cập nhật thành công (204 NoContent)
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] Product product)
        {
            // Kiểm tra product và id có hợp lệ không
            if (product == null || id != product.Id || _dbContext.Categories.FirstOrDefault(s => s.Id == product.CategoryId) == null)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm product theo id
            var pro = await _dbContext.Products.FirstOrDefaultAsync(s => s.Id == id);
            // Cập nhật thông tin product
            pro.Name = product.Name;
            // Lưu thay đổi vào database
            _dbContext.SaveChanges();
            // Trả về phản hồi cập nhật thành công (204 NoContent)
            return NoContent();
        }
    }
}