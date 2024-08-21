using Microsoft.AspNetCore.Mvc;
using TheFirstTask.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using TheFirstTask.Model;
using Microsoft.AspNetCore.Authorization;
using System.Drawing.Printing;

namespace TheFirstTask.Controllers
{
    [Authorize(Roles = "LineLeader")]
    [Route("api/TaskOrderDetail")]
    [ApiController]
    public class TaskOrderDetailController : ControllerBase
    {
        private readonly AppDBContext _dBcontext;
        public TaskOrderDetailController(AppDBContext dBContext)
        {
            _dBcontext = dBContext;
        }

        [HttpGet(Name = "GetTaskOrderDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<TaskOrderDetail>>> GetTaskOrderDetails(int page = 1, int pageSize = 10)
        {
            var totalCount = await _dBcontext.TaskOrderDetails.CountAsync();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            var taskorderdetailsPerPage = await _dBcontext.TaskOrderDetails.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(taskorderdetailsPerPage);
        }

        [HttpGet("{id:int}", Name = "GetTaskOrderDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskOrderDetail>> GetTaskOrderDetail(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var taskOrderDetail = await _dBcontext.TaskOrderDetails.FirstOrDefaultAsync(s => s.Id == id);
            if (taskOrderDetail == null)
            {
                return NotFound();
            }
            return Ok(taskOrderDetail);
        }

        [HttpPost(Name = "CreateTaskOrderDetail")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TaskOrderDetail>> CreateTaskOrderDetail([FromBody] TaskOrderDetail taskOrderDetail)
        {
            var chkIdTask = await _dBcontext.TaskOrders.FirstOrDefaultAsync(s => s.Id == taskOrderDetail.TaskId);
            var chkIdOrder = await _dBcontext.Orders.FirstOrDefaultAsync(s => s.Id == taskOrderDetail.OrderId);
            if (taskOrderDetail == null || chkIdOrder == null || chkIdTask == null)
            {
                return BadRequest(taskOrderDetail);
            }
            if (taskOrderDetail.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            _dBcontext.TaskOrderDetails.Add(taskOrderDetail);
            _dBcontext.SaveChanges();
            return CreatedAtRoute("GetTaskOrderDetail", new { id = taskOrderDetail.Id }, taskOrderDetail);
        }

        [HttpDelete("{id:int}", Name = "DeleteTaskOrderDetail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTaskOrderDetail(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var taskOrderDetail = await _dBcontext.TaskOrderDetails.FirstOrDefaultAsync(s => s.Id == id);
            if (taskOrderDetail == null)
            {
                return NotFound();
            }
            _dBcontext.TaskOrderDetails.Remove(taskOrderDetail);
            _dBcontext.SaveChanges();
            return Ok();
        }

        [HttpPut("{id:int}", Name = "UpdateTaskOrderDetail")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateTaskOrderDetail(int id, [FromBody] TaskOrderDetail taskOrderDetail)
        {
            var chkIdTask = await _dBcontext.TaskOrders.FirstOrDefaultAsync(s => s.Id == taskOrderDetail.TaskId);
            var chkIdOrder = await _dBcontext.Orders.FirstOrDefaultAsync(s => s.Id == taskOrderDetail.OrderId);
            if (taskOrderDetail == null || id != taskOrderDetail.Id || chkIdTask == null || chkIdOrder == null)
            {
                return BadRequest();
            }

            var existingTaskOrderDetail = await _dBcontext.TaskOrderDetails.FirstOrDefaultAsync(s => s.Id == id);
            if (existingTaskOrderDetail == null)
            {
                return NotFound();
            }

            existingTaskOrderDetail.TaskId = taskOrderDetail.TaskId;
            existingTaskOrderDetail.OrderId = taskOrderDetail.OrderId;

            _dBcontext.SaveChanges();
            return Ok();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialTaskOrderDetail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePartialTaskOrderDetail(int id, JsonPatchDocument<TaskOrderDetail> taskOrderDetail)
        {
            if (taskOrderDetail == null || id == 0)
            {
                return BadRequest();
            }
            var taskOrder = await _dBcontext.TaskOrderDetails.FirstOrDefaultAsync(o => o.Id == id);
            if (taskOrder == null)
            {
                return NotFound();
            }
            taskOrderDetail.ApplyTo(taskOrder, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _dBcontext.SaveChanges();
            return NoContent();
        }
    }
}