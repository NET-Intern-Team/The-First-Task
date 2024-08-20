using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheFirstTask.Data;
using TheFirstTask.Model;

namespace TheFirstTask.Controllers
{
    [Route("api/Category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDBContext _dBcontext;
        public CategoryController(AppDBContext dBContext)
        {
            _dBcontext = dBContext;
        }
        [HttpGet(Name = "GetCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<Category>>> GetCategories()
        {
            return Ok(_dBcontext.Categories.ToList());
        }

        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var cate = await _dBcontext.Categories.FirstOrDefaultAsync(s => s.Id == id);
            if (cate == null)
            {
                return NotFound();
            }
            return Ok(cate);
        }

        [HttpPost(Name = "CreateCategory")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {
            if (_dBcontext.Categories.FirstOrDefault(s => s.Name.ToLower() == category.Name.ToLower()) != null)
            {
                ModelState.AddModelError("ErrorNameDuplicate", "Category name already existed");
                return BadRequest(ModelState);
            }
            if (category == null)
            {
                return BadRequest(category);
            }
            if (category.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            _dBcontext.Categories.Add(category);
            _dBcontext.SaveChanges();
            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        }

        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            //IActionResult đc sử dụng khi không có kiểu dữ liệu trả về đối với request nhận được
            //NoContent with status code 204
            if (id == 0)
            {
                return BadRequest();
            }
            var cate = await _dBcontext.Categories.FirstOrDefaultAsync(s => s.Id == id);
            if (cate == null)
            {
                return NotFound();
            }
            _dBcontext.Categories.Remove(cate);
            _dBcontext.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateCategory")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]Category category)
        {
            if (category == null || id != category.Id)
            {
                return BadRequest();
            }
            var cate = await _dBcontext.Categories.FirstOrDefaultAsync(s => s.Id == id);
            cate.Name = category.Name;
            _dBcontext.SaveChanges();
            return NoContent();
        }
    }
}
