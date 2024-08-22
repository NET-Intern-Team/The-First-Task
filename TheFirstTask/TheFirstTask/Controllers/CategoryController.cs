using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheFirstTask.Data;
using TheFirstTask.Model;

namespace TheFirstTask.Controllers
{
    // Định nghĩa route cho controller
    [Route("api/Category")]
    [ApiController] // Xác định đây là một controller API
    public class CategoryController : ControllerBase
    {
        private readonly AppDBContext _dBcontext; // Biến lưu trữ context của database

        // Khởi tạo context trong constructor
        public CategoryController(AppDBContext dBContext)
        {
            _dBcontext = dBContext;
        }

        // Endpoint để lấy danh sách các categories
        [HttpGet(Name = "GetCategories")] // Xác định đây là method GET
        [ProducesResponseType(StatusCodes.Status200OK)] // Xác định loại phản hồi thành công (200 OK)
        public async Task<ActionResult<ICollection<Category>>> GetCategories(int page = 1, int pageSize = 10)
        {
            // Lấy tổng số categories
            var totalCount = await _dBcontext.Categories.CountAsync();
            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            // Lấy danh sách categories trên trang hiện tại
            var categoriesPerPage = await _dBcontext.Categories.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            // Trả về danh sách categories
            return Ok(categoriesPerPage);
        }

        // Endpoint để lấy thông tin chi tiết của một category
        [HttpGet("{id:int}", Name = "GetCategory")] // Xác định đây là method GET, id là một số nguyên
        [ProducesResponseType(StatusCodes.Status200OK)] // Xác định loại phản hồi thành công (200 OK)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Xác định loại phản hồi không tìm thấy (404 NotFound)
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            // Kiểm tra id có hợp lệ không
            if (id == 0)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm category theo id
            var cate = await _dBcontext.Categories.FirstOrDefaultAsync(s => s.Id == id);
            // Kiểm tra category có tồn tại không
            if (cate == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Trả về thông tin category
            return Ok(cate);
        }

        // Endpoint để tạo một category mới
        [HttpPost(Name = "CreateCategory")] // Xác định đây là method POST
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status201Created)] // Xác định loại phản hồi tạo thành công (201 Created)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Xác định loại phản hồi lỗi server (500 InternalServerError)
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {
            // Kiểm tra tên category đã tồn tại chưa
            if (_dBcontext.Categories.FirstOrDefault(s => s.Name.ToLower() == category.Name.ToLower()) != null)
            {
                // Thêm lỗi vào ModelState
                ModelState.AddModelError("ErrorNameDuplicate", "Category name already existed");
                // Trả về lỗi 400 BadRequest
                return BadRequest(ModelState);
            }
            // Kiểm tra category có hợp lệ không
            if (category == null)
            {
                return BadRequest(category); // Trả về lỗi 400 BadRequest
            }
            // Kiểm tra id có hợp lệ không
            if (category.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError); // Trả về lỗi 500 InternalServerError
            }
            // Thêm category vào database
            _dBcontext.Categories.Add(category);
            _dBcontext.SaveChanges();
            // Trả về thông tin category mới tạo
            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        }

        // Endpoint để xóa một category
        [HttpDelete("{id:int}", Name = "DeleteCategory")] // Xác định đây là method DELETE, id là một số nguyên
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Xác định loại phản hồi xóa thành công (204 NoContent)
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Xác định loại phản hồi không tìm thấy (404 NotFound)
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // Kiểm tra id có hợp lệ không
            if (id == 0)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm category theo id
            var cate = await _dBcontext.Categories.FirstOrDefaultAsync(s => s.Id == id);
            // Kiểm tra category có tồn tại không
            if (cate == null)
            {
                return NotFound(); // Trả về lỗi 404 NotFound
            }
            // Xóa category khỏi database
            _dBcontext.Categories.Remove(cate);
            _dBcontext.SaveChanges();
            // Trả về phản hồi xóa thành công (204 NoContent)
            return NoContent();
        }

        // Endpoint để cập nhật thông tin của một category
        [HttpPut("{id:int}", Name = "UpdateCategory")] // Xác định đây là method PUT, id là một số nguyên
        [Authorize(Roles = "admin")] // Yêu cầu quyền "admin" để truy cập
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Xác định loại phản hồi lỗi (400 BadRequest)
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Xác định loại phản hồi cập nhật thành công (204 NoContent)
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] Category category)
        {
            // Kiểm tra category và id có hợp lệ không
            if (category == null || id != category.Id)
            {
                return BadRequest(); // Trả về lỗi 400 BadRequest
            }
            // Tìm category theo id
            var cate = await _dBcontext.Categories.FirstOrDefaultAsync(s => s.Id == id);
            // Cập nhật thông tin category
            cate.Name = category.Name;
            // Lưu thay đổi vào database
            _dBcontext.SaveChanges();
            // Trả về phản hồi cập nhật thành công (204 NoContent)
            return NoContent();
        }
    }
}