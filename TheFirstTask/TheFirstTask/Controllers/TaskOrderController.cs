using Microsoft.AspNetCore.Mvc;
using TheFirstTask.Data;
using Microsoft.EntityFrameworkCore;
using TheFirstTask.Model;
using Microsoft.AspNetCore.Authorization;

namespace TheFirstTask.Controllers
{
    [Route("api/TaskOrder")]
    [ApiController]
    public class TaskOrderController : ControllerBase
    {
        private readonly AppDBContext _dbContext;
        public TaskOrderController(AppDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        [HttpGet(Name = "GetTaskOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<TaskOrder>>> GetTaskOrders()
        {
            return Ok(_dbContext.TaskOrders.ToList());
        }

        [HttpGet("{id:int}", Name = "GetTaskOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskOrder>> GetTaskOrder(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var task = await _dbContext.TaskOrders.FirstOrDefaultAsync(s => s.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost(Name = "CreateTaskOrder")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TaskOrderDetail>> CreateTaskOrder([FromBody] TaskOrder task)
        {
            if (task == null)
            {
                return BadRequest(task);
            }
            if (task.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            _dbContext.TaskOrders.Add(task);
            _dbContext.SaveChanges();
            return CreatedAtRoute("GetTaskOrder", new { id = task.Id }, task);
        }

        [HttpDelete("{id:int}", Name = "DeleteTaskOrder")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTaskOrder(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var task = await _dbContext.TaskOrders.FirstOrDefaultAsync(s => s.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            _dbContext.TaskOrders.Remove(task);
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateTaskOrder")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateTaskOrder(int id, [FromBody] TaskOrder task)
        {
            if (task == null || id != task.Id)
            {
                return BadRequest();
            }

            var taskorder = await _dbContext.TaskOrders.FirstOrDefaultAsync(s => s.Id == id);
            if (taskorder == null)
            {
                return NotFound();
            }

            taskorder.TaskContent = taskorder.TaskContent;

            _dbContext.SaveChanges();
            return NoContent();
        }

      
    }
}