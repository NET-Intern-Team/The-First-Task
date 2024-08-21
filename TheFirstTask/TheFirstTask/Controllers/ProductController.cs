using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheFirstTask.Data;
using TheFirstTask.Model;

namespace TheFirstTask.Controllers
{
    //Comment
    [Route("api/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDBContext _dbContext;
        public ProductController(AppDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        [HttpGet(Name = "GetProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<Product>>> GetProducts(int page = 1, int pageSize = 10)
        {
            var totalCount = await _dbContext.Products.CountAsync();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            var productsPerPage = await _dbContext.Products.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(productsPerPage);
        }


        [HttpGet("{id:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var pro = await _dbContext.Products.FirstOrDefaultAsync(s => s.Id == id);
            if (pro == null)
            {
                return NotFound();
            }
            return Ok(pro);

        }

        [HttpPost(Name = "CreateProduct")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (_dbContext.Products.FirstOrDefault(s => s.Name.ToLower() == product.Name.ToLower()) != null)
            {
                ModelState.AddModelError("ErrorNameDuplicate", "Product name already existed");
                return BadRequest(ModelState);
            }
            if (product == null || _dbContext.Categories.FirstOrDefault(s=> s.Id == product.CategoryId) == null)
            {
                return BadRequest(product);
            }
            if (product.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpDelete("{id:int}", Name = "DeleteProduct")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {

            if (id == 0)
            {
                return BadRequest();
            }
            var pro = await _dbContext.Products.FirstOrDefaultAsync(s => s.Id == id);
            if (pro == null)
            {
                return NotFound();
            }
            _dbContext.Products.Remove(pro);
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateProduct")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] Product product)
        {
            if (product == null || id != product.Id || _dbContext.Categories.FirstOrDefault(s => s.Id == product.CategoryId) == null)
            {
                return BadRequest();
            }
            var pro = await _dbContext.Products.FirstOrDefaultAsync(s => s.Id == id);
            pro.Name = product.Name;
            _dbContext.SaveChanges();
            return NoContent();
        }
    }
}
