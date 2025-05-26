using System.ComponentModel.DataAnnotations;

namespace Task_Management.DTO
{
	public class TaskItemDto
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public bool IsCompleted { get; set; }
		[Required]
		public string Priority { get; set; } // High, Medium, Low



	}
}
