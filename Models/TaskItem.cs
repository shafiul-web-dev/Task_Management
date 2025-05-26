using System.ComponentModel.DataAnnotations;

namespace Task_Management.Models
{
	public class TaskItem
	{
		[Key]
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public bool IsCompleted { get; set; }

		//FIELD: Priority (High, Medium, Low)
		[Required]
		public string Priority { get; set; }
	}

}
