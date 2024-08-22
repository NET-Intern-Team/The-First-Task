using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheFirstTask.Data;
using TheFirstTask.Model;

namespace TheFirstTask.Controllers
{
    // Định nghĩa route cho controller
    [Route("api/TaskOrder")]
    [ApiController] // Xác định đây là một controller API
    public class TaskOrderController : ControllerBase
    {
        private readonly AppDBContext _dbContext; // Biến lưu trữ context của database

        // Khởi tạo context trong constructor
        public TaskOrderController(AppDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        // Endpoint để lấy danh sách các taskorders
        [HttpGet(Name = "GetTaskOrders")] // Xác định đây là method GET
        [ProducesResponseType(StatusCodes.Status200OK)] // Xác định loại phản hồi thành công (200 OK)
        public async Task<ActionResult<ICollection<TaskOrder>>> GetTaskOrders(int page = 1, int pageSize = 10)
        {
            // Lấy tổng số taskorders
            var totalCount = await _dbContext.TaskOrders.CountAsync();
            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            // Lấy danh sách taskorders trên trang hiện tại
            var taskordersPerPage = await _dbContext.TaskOrders.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            // Trả về danh sách taskorders
            return Ok(taskordersPerPage);
        }

        // Endpoint để lấy thông tin chi tiết của một taskorder
        [HttpGet("{id:int}", Name = "GetTaskOrder")] // Xác định đây là method GET, id là một số nguyên
        [ProducesResponseType(StatusCodes.Status200OK)] // Xác định loại phản hồi thành công (200 OK)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Xác định loại phản hồi không tìm thấy (404 NotFound)
        public async Task<ActionResult<TaskOrder>> GetTaskOrder(int id)
        {
            // Kiểm tra id có hợp lệ không
            if (id == 0)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm taskorder theo id
            var task = await _dbContext.TaskOrders.FirstOrDefaultAsync(s => s.Id == id);
            // Kiểm tra taskorder có tồn tại không
            if (task == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Trả về thông tin taskorder
            return Ok(task);
        }

        // Endpoint để tạo một taskorder mới
        [HttpPost(Name = "CreateTaskOrder")] // Xác định đây là method POST
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status201Created)] // Xác định loại phản hồi tạo thành công (201 Created)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Xác định loại phản hồi lỗi server (500 InternalServerError)
        public async Task<ActionResult<TaskOrderDetail>> CreateTaskOrder([FromBody] TaskOrder task)
        {
            // Kiểm tra taskorder có hợp lệ không
            if (task == null)
            {
                return BadRequest(task); // Trả về lỗi 400 BadRequest
            }
            // Kiểm tra id có hợp lệ không
            if (task.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError); // Trả về lỗi 500 InternalServerError
            }
            // Thêm taskorder vào database
            _dbContext.TaskOrders.Add(task);
            _dbContext.SaveChanges();
            // Trả về thông tin taskorder mới tạo
            return CreatedAtRoute("GetTaskOrder", new { id = task.Id }, task);
        }

        // Endpoint để xóa một taskorder
        [HttpDelete("{id:int}", Name = "DeleteTaskOrder")] // Xác định đây là method DELETE, id là một số nguyên
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Xác định loại phản hồi xóa thành công (204 NoContent)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Xác định loại phản hồi không tìm thấy (404 NotFound)
        public async Task<IActionResult> DeleteTaskOrder(int id)
        {
            // Kiểm tra id có hợp lệ không
            if (id == 0)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm taskorder theo id
            var task = await _dbContext.TaskOrders.FirstOrDefaultAsync(s => s.Id == id);
            // Kiểm tra taskorder có tồn tại không
            if (task == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Xóa taskorder khỏi database
            _dbContext.TaskOrders.Remove(task);
            _dbContext.SaveChanges();
            // Trả về phản hồi xóa thành công (204 NoContent)
            return NoContent();
        }

        // Endpoint để cập nhật thông tin của một taskorder
        [HttpPut("{id:int}", Name = "UpdateTaskOrder")] // Xác định đây là method PUT, id là một số nguyên
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Xác định loại phản hồi cập nhật thành công (204 NoContent)
        public async Task<IActionResult> UpdateTaskOrder(int id, [FromBody] TaskOrder task)
        {
            // Kiểm tra taskorder và id có hợp lệ không
            if (task == null || id != task.Id)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm taskorder theo id
            var taskorder = await _dbContext.TaskOrders.FirstOrDefaultAsync(s => s.Id == id);
            // Kiểm tra taskorder có tồn tại không
            if (taskorder == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Cập nhật thông tin taskorder
            taskorder.TaskContent = task.TaskContent; // Cập nhật nội dung công việc
            // Lưu thay đổi vào database
            _dbContext.SaveChanges();
            // Trả về phản hồi cập nhật thành công (204 NoContent)
            return NoContent();
        }

    }
}