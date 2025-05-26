using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Management.Data;
using Task_Management.DTO;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

		// GET: api/TaskItems
		[HttpGet]
		public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			return await _context.TaskItems
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}

		// GET: api/TaskItems/5
		[HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskItem(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);

            if (taskItem == null)
            {
                return NotFound();
            }

            return taskItem;
        }

        // PUT: api/TaskItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskItem(int id, TaskItem taskItem)
        {
            if (id != taskItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(taskItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
			catch (DbUpdateConcurrencyException)
			{
				if (!TaskItemExists(id))
				{
					return NotFound(new { message = "Task item not found." });
				}
				return StatusCode(500, new { message = "Database error occurred." });
			}

			return NoContent();
        }

		// POST: api/TaskItems
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<TaskItem>> PostTaskItem(TaskItemDto taskDto)
		{
			var taskItem = new TaskItem
			{
				Title = taskDto.Title,
				Description = taskDto.Description,
				IsCompleted = false
			};

			_context.TaskItems.Add(taskItem);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, taskItem);
		}

		// DELETE: api/TaskItems/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTaskItem(int id)
		{
			var taskItem = await _context.TaskItems.FindAsync(id);
			if (taskItem == null)
			{
				return NotFound(new { message = "Task not found." });
			}

			_context.TaskItems.Remove(taskItem);
			await _context.SaveChangesAsync();

			return Ok(new { message = "Task deleted successfully." });
		}

		private bool TaskItemExists(int id)
        {
            return _context.TaskItems.Any(e => e.Id == id);
        }
    }
}
