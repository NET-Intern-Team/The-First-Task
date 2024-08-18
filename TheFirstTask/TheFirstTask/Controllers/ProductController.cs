using Microsoft.AspNetCore.Mvc;
using TheFirstTask.Data;
using TheFirstTask.Model;

namespace TheFirstTask.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDBContext _dbContext;
        public ProductController(AppDBContext dBContext) { 
            _dbContext = dBContext;
        }

        [HttpGet(Name = "GetProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<Product>>> GetProducts()
        {
            return Ok(_dbContext.Products.ToList());
        }
    }
}
