using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheFirstTask.Data;
using TheFirstTask.Model;

namespace TheFirstTask.Controllers
{
    [Route("api/Order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDBContext _dBcontext;
        public OrderController(AppDBContext dBContext)
        {
            _dBcontext = dBContext;
        }

        [HttpGet(Name = "GetOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<Order>>> GetOrders()
        {
            return Ok(_dBcontext.Orders.ToList());
        }

        [HttpGet("{id:int}", Name = "GetOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var order = await _dBcontext.Orders.FirstOrDefaultAsync(s => s.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost(Name = "CreateOrder")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest(order);
            }
            if (order.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            _dBcontext.Orders.Add(order);
            _dBcontext.SaveChanges();
            return CreatedAtRoute("GetOrder", new { id = order.Id }, order);
        }

        [HttpDelete("{id:int}", Name = "DeleteOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            //IActionResult được sử dụng khi không có kiểu dữ liệu trả về đối với request nhận được
            //NoContent with status code 204
            if (id == 0)
            {
                return BadRequest();
            }
            var order = await _dBcontext.Orders.FirstOrDefaultAsync(s => s.Id == id);//Lấy order từ database dựa trên ID cung cấp
            if (order == null)
            {
                return NotFound();
            }
            _dBcontext.Orders.Remove(order);//Xóa order
            _dBcontext.SaveChanges();
            return Ok();
        }

        [HttpPut("{id:int}", Name = "UpdateOrder")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (order == null || id != order.Id)
            {
                return BadRequest();
            }

            var existingOrder = await _dBcontext.Orders.FirstOrDefaultAsync(s => s.Id == id);//Lấy order từ database dựa trên ID cung cấp
            if (existingOrder == null)
            {
                return NotFound();
            }

            // Kiểm tra giá trị null cho ProductId và Quantity
            if (order.ProductId == 0 || order.Quantity <= 0)
            {
                return BadRequest("Vui lòng cung cấp thông tin ProductId và Quantity.");
            }

            existingOrder.ProductId = order.ProductId;//Cập nhật
            existingOrder.Quantity = order.Quantity;

            _dBcontext.SaveChanges();
            return Ok();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePartialOrder(int id, JsonPatchDocument<Order> order)
        {
            if(order == null || id == 0)
            {
                return BadRequest();
            }
            var ord = await _dBcontext.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if(ord == null)
            {
                return NotFound();
            }
            order.ApplyTo(ord, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _dBcontext.SaveChanges();
            return NoContent();
        }
        
    }
}