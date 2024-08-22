using Microsoft.AspNetCore.Mvc;
using TheFirstTask.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using TheFirstTask.Model;
using Microsoft.AspNetCore.Authorization;

namespace TheFirstTask.Controllers
{
    // Định nghĩa route cho controller
    [Authorize(Roles = "LineLeader")] // Yêu cầu quyền "LineLeader" để truy cập
    [Route("api/TaskOrderDetail")]
    [ApiController] // Xác định đây là một controller API
    public class TaskOrderDetailController : ControllerBase
    {
        private readonly AppDBContext _dBcontext; // Biến lưu trữ context của database

        // Khởi tạo context trong constructor
        public TaskOrderDetailController(AppDBContext dBContext)
        {
            _dBcontext = dBContext;
        }

        // Endpoint để lấy danh sách các taskorderdetails
        [HttpGet(Name = "GetTaskOrderDetails")] // Xác định đây là method GET
        [ProducesResponseType(StatusCodes.Status200OK)] // Xác định loại phản hồi thành công (200 OK)
        public async Task<ActionResult<ICollection<TaskOrderDetail>>> GetTaskOrderDetails(int page = 1, int pageSize = 10)
        {
            // Lấy tổng số taskorderdetails
            var totalCount = await _dBcontext.TaskOrderDetails.CountAsync();
            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            // Lấy danh sách taskorderdetails trên trang hiện tại
            var taskorderdetailsPerPage = await _dBcontext.TaskOrderDetails.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            // Trả về danh sách taskorderdetails
            return Ok(taskorderdetailsPerPage);
        }

        // Endpoint để lấy thông tin chi tiết của một taskorderdetail
        [HttpGet("{id:int}", Name = "GetTaskOrderDetail")] // Xác định đây là method GET, id là một số nguyên
        [ProducesResponseType(StatusCodes.Status200OK)] // Xác định loại phản hồi thành công (200 OK)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Xác định loại phản hồi không tìm thấy (404 NotFound)
        public async Task<ActionResult<TaskOrderDetail>> GetTaskOrderDetail(int id)
        {
            // Kiểm tra id có hợp lệ không
            if (id == 0)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm taskorderdetail theo id
            var taskOrderDetail = await _dBcontext.TaskOrderDetails.FirstOrDefaultAsync(s => s.Id == id);
            // Kiểm tra taskorderdetail có tồn tại không
            if (taskOrderDetail == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Trả về thông tin taskorderdetail
            return Ok(taskOrderDetail);
        }

        // Endpoint để tạo một taskorderdetail mới
        [HttpPost(Name = "CreateTaskOrderDetail")] // Xác định đây là method POST
        [ProducesResponseType(StatusCodes.Status201Created)] // Xác định loại phản hồi tạo thành công (201 Created)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Xác định loại phản hồi lỗi server (500 InternalServerError)
        public async Task<ActionResult<TaskOrderDetail>> CreateTaskOrderDetail([FromBody] TaskOrderDetail taskOrderDetail)
        {
            // Kiểm tra taskorder và order có tồn tại không
            var chkIdTask = await _dBcontext.TaskOrders.FirstOrDefaultAsync(s => s.Id == taskOrderDetail.TaskId);
            var chkIdOrder = await _dBcontext.Orders.FirstOrDefaultAsync(s => s.Id == taskOrderDetail.OrderId);
            // Kiểm tra taskorderdetail có hợp lệ không
            if (taskOrderDetail == null || chkIdOrder == null || chkIdTask == null)
            {
                return BadRequest(taskOrderDetail); // Trả về lỗi 400 BadRequest
            }
            // Kiểm tra id có hợp lệ không
            if (taskOrderDetail.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError); // Trả về lỗi 500 InternalServerError
            }
            // Thêm taskorderdetail vào database
            _dBcontext.TaskOrderDetails.Add(taskOrderDetail);
            _dBcontext.SaveChanges();
            // Trả về thông tin taskorderdetail mới tạo
            return CreatedAtRoute("GetTaskOrderDetail", new { id = taskOrderDetail.Id }, taskOrderDetail);
        }

        // Endpoint để xóa một taskorderdetail
        [HttpDelete("{id:int}", Name = "DeleteTaskOrderDetail")] // Xác định đây là method DELETE, id là một số nguyên
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Xác định loại phản hồi xóa thành công (204 NoContent)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Xác định loại phản hồi không tìm thấy (404 NotFound)
        public async Task<IActionResult> DeleteTaskOrderDetail(int id)
        {
            // Kiểm tra id có hợp lệ không
            if (id == 0)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm taskorderdetail theo id
            var taskOrderDetail = await _dBcontext.TaskOrderDetails.FirstOrDefaultAsync(s => s.Id == id);
            // Kiểm tra taskorderdetail có tồn tại không
            if (taskOrderDetail == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Xóa taskorderdetail khỏi database
            _dBcontext.TaskOrderDetails.Remove(taskOrderDetail);
            _dBcontext.SaveChanges();
            // Trả về phản hồi xóa thành công (204 NoContent)
            return NoContent();
        }

        // Endpoint để cập nhật thông tin của một taskorderdetail
        [HttpPut("{id:int}", Name = "UpdateTaskOrderDetail")] // Xác định đây là method PUT, id là một số nguyên
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Xác định loại phản hồi cập nhật thành công (204 NoContent)
        public async Task<IActionResult> UpdateTaskOrderDetail(int id, [FromBody] TaskOrderDetail taskOrderDetail)
        {
            // Kiểm tra taskorder và order có tồn tại không
            var chkIdTask = await _dBcontext.TaskOrders.FirstOrDefaultAsync(s => s.Id == taskOrderDetail.TaskId);
            var chkIdOrder = await _dBcontext.Orders.FirstOrDefaultAsync(s => s.Id == taskOrderDetail.OrderId);
            // Kiểm tra taskorderdetail và id có hợp lệ không
            if (taskOrderDetail == null || id != taskOrderDetail.Id || chkIdTask == null || chkIdOrder == null)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm taskorderdetail theo id
            var existingTaskOrderDetail = await _dBcontext.TaskOrderDetails.FirstOrDefaultAsync(s => s.Id == id);
            // Kiểm tra taskorderdetail có tồn tại không
            if (existingTaskOrderDetail == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Cập nhật thông tin taskorderdetail
            existingTaskOrderDetail.TaskId = taskOrderDetail.TaskId;
            existingTaskOrderDetail.OrderId = taskOrderDetail.OrderId;
            // Lưu thay đổi vào database
            _dBcontext.SaveChanges();
            // Trả về phản hồi cập nhật thành công (204 NoContent)
            return NoContent();
        }

        // Endpoint để cập nhật một phần thông tin của một taskorderdetail
        [HttpPatch("{id:int}", Name = "UpdatePartialTaskOrderDetail")] // Xác định đây là method PATCH, id là một số nguyên
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Xác định loại phản hồi cập nhật thành công (204 NoContent)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Xác định loại phản hồi không tìm thấy (404 NotFound)
        public async Task<IActionResult> UpdatePartialTaskOrderDetail(int id, JsonPatchDocument<TaskOrderDetail> taskOrderDetail)
        {
            // Kiểm tra taskorderdetail và id có hợp lệ không
            if (taskOrderDetail == null || id == 0)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm taskorderdetail theo id
            var taskOrder = await _dBcontext.TaskOrderDetails.FirstOrDefaultAsync(o => o.Id == id);
            // Kiểm tra taskorderdetail có tồn tại không
            if (taskOrder == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Áp dụng JsonPatchDocument vào taskorderdetail
            taskOrderDetail.ApplyTo(taskOrder, ModelState); 
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