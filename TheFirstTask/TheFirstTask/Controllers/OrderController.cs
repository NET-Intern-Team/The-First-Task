using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheFirstTask.Data;
using TheFirstTask.Model;

namespace TheFirstTask.Controllers
{
    // Định nghĩa route cho controller
    [Route("api/Order")]
    [ApiController] // Xác định đây là một controller API
    public class OrderController : ControllerBase
    {
        private readonly AppDBContext _dBcontext; // Biến lưu trữ context của database

        // Khởi tạo context trong constructor
        public OrderController(AppDBContext dBContext)
        {
            _dBcontext = dBContext;
        }

        // Endpoint để lấy danh sách các orders
        [HttpGet(Name = "GetOrders")] // Xác định đây là method GET
        [ProducesResponseType(StatusCodes.Status200OK)] // Xác định loại phản hồi thành công (200 OK)
        public async Task<ActionResult<ICollection<Order>>> GetOrders(int page = 1, int pageSize = 10)
        {
            // Lấy tổng số orders
            var totalCount = await _dBcontext.Orders.CountAsync();
            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            // Lấy danh sách orders trên trang hiện tại
            var ordersPerPage = await _dBcontext.Orders.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            // Trả về danh sách orders
            return Ok(ordersPerPage);
        }

        // Endpoint để lấy thông tin chi tiết của một order
        [HttpGet("{id:int}", Name = "GetOrder")] // Xác định đây là method GET, id là một số nguyên
        [ProducesResponseType(StatusCodes.Status200OK)] // Xác định loại phản hồi thành công (200 OK)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Xác định loại phản hồi không tìm thấy (404 NotFound)
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            // Kiểm tra id có hợp lệ không
            if (id == 0)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm order theo id
            var order = await _dBcontext.Orders.FirstOrDefaultAsync(s => s.Id == id);
            // Kiểm tra order có tồn tại không
            if (order == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Trả về thông tin order
            return Ok(order);
        }

        // Endpoint để tạo một order mới
        [HttpPost(Name = "CreateOrder")] // Xác định đây là method POST
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status201Created)] // Xác định loại phản hồi tạo thành công (201 Created)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Xác định loại phản hồi lỗi server (500 InternalServerError)
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            // Kiểm tra order và product có hợp lệ không
            if (order == null || _dBcontext.Products.FirstOrDefault(s => s.Id == order.ProductId) == null)
            {
                return BadRequest(order); // Trả về lỗi 400 BadRequest
            }
            // Kiểm tra id có hợp lệ không
            if (order.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError); // Trả về lỗi 500 InternalServerError
            }
            // Thêm order vào database
            _dBcontext.Orders.Add(order);
            _dBcontext.SaveChanges();
            // Trả về thông tin order mới tạo
            return CreatedAtRoute("GetOrder", new { id = order.Id }, order);
        }

        // Endpoint để xóa một order
        [HttpDelete("{id:int}", Name = "DeleteOrder")] // Xác định đây là method DELETE, id là một số nguyên
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Xác định loại phản hồi xóa thành công (204 NoContent)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Xác định loại phản hồi không tìm thấy (404 NotFound)
        public async Task<IActionResult> DeleteOrder(int id)
        {
            // Kiểm tra id có hợp lệ không
            if (id == 0)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm order theo id
            var order = await _dBcontext.Orders.FirstOrDefaultAsync(s => s.Id == id); // Lấy order từ database dựa trên ID cung cấp
            // Kiểm tra order có tồn tại không
            if (order == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Xóa order khỏi database
            _dBcontext.Orders.Remove(order); // Xóa order
            _dBcontext.SaveChanges();
            // Trả về phản hồi xóa thành công (204 NoContent)
            return NoContent();
        }

        // Endpoint để cập nhật thông tin của một order
        [HttpPut("{id:int}", Name = "UpdateOrder")] // Xác định đây là method PUT, id là một số nguyên
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Xác định loại phản hồi cập nhật thành công (204 NoContent)
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            // Kiểm tra order và id có hợp lệ không
            if (order == null || id != order.Id || _dBcontext.Products.FirstOrDefault(s => s.Id == order.ProductId) == null)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm order theo id
            var existingOrder = await _dBcontext.Orders.FirstOrDefaultAsync(s => s.Id == id); // Lấy order từ database dựa trên ID cung cấp
            // Kiểm tra order có tồn tại không
            if (existingOrder == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Kiểm tra ProductId và Quantity có hợp lệ không
            if (order.ProductId == 0 || order.Quantity <= 0)
            {
                return BadRequest("Vui lòng cung cấp thông tin ProductId và Quantity.");
            }
            // Cập nhật thông tin order
            existingOrder.ProductId = order.ProductId; // Cập nhật
            existingOrder.Quantity = order.Quantity;
            // Lưu thay đổi vào database
            _dBcontext.SaveChanges();
            // Trả về phản hồi cập nhật thành công (204 NoContent)
            return NoContent();
        }

        // Endpoint để cập nhật một phần thông tin của một order
        [HttpPatch("{id:int}", Name = "UpdatePartialOrder")] // Xác định đây là method PATCH, id là một số nguyên
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Xác định loại phản hồi cập nhật thành công (204 NoContent)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Xác định loại phản hồi không tìm thấy (404 NotFound)
        public async Task<IActionResult> UpdatePartialOrder(int id, JsonPatchDocument<Order> order)
        {
            // Kiểm tra order và id có hợp lệ không
            if (order == null || id == 0)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm order theo id
            var ord = await _dBcontext.Orders.FirstOrDefaultAsync(o => o.Id == id);
            // Kiểm tra order có tồn tại không
            if (ord == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Áp dụng JsonPatchDocument vào order
            order.ApplyTo(ord, ModelState);
            // Kiểm tra ModelState có hợp lệ không
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi 400 BadRequest
            }
            // Lưu thay đổi vào database
            _dBcontext.SaveChanges();
            // Trả về phản hồi cập nhật thành công (204 NoContent)
            return NoContent();
        }
    }
}