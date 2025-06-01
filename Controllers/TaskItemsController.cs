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


		[HttpGet]
		public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItems(
			[FromQuery] string priority = "All",
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			var query = _context.TaskItems.AsQueryable();

			if (priority != "All")
			{
				query = query.Where(t => t.Priority == priority);
			}

			var totalRecords = await query.CountAsync();
			var taskItems = await query
				.OrderBy(t => t.Priority)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return Ok(new
			{
				TotalRecords = totalRecords,
				PageNumber = pageNumber,
				PageSize = pageSize,
				Data = taskItems
			});
		}

		
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

		
		[HttpPut("{id}")]
		public async Task<IActionResult> PutTaskItem(int id, TaskItemDto taskDto)
		{
			var taskItem = await _context.TaskItems.FindAsync(id);

			if (taskItem == null)
			{
				return NotFound(new { message = "Task item not found." });
			}

			
			taskItem.Title = taskDto.Title;
			taskItem.Description = taskDto.Description;
			taskItem.IsCompleted = taskDto.IsCompleted;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				return StatusCode(500, new { message = "Database error occurred." });
			}

			return Ok(new { message = "Task updated successfully." });
		}

		[HttpPost]
		public async Task<ActionResult<TaskItem>> PostTaskItem(TaskItemDto taskDto)
		{
			if (taskDto.Priority != "High" && taskDto.Priority != "Medium" && taskDto.Priority != "Low")
			{
				return BadRequest(new { message = "Priority must be High, Medium, or Low." });
			}

			var taskItem = new TaskItem
			{
				Title = taskDto.Title,
				Description = taskDto.Description,
				Priority = taskDto.Priority,
				IsCompleted = false
			};

			_context.TaskItems.Add(taskItem);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, taskItem);
		}

		
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
