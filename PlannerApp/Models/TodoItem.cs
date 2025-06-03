using System.ComponentModel.DataAnnotations;

namespace PlannerApp.Models
{
    // Represents a single to-do task in our application
    public class TodoItem
    {
        // The primary key for the database table
        // EF Core will treat this as the unique identifier
        [Key]
        public int Id { get; set; }

        // The title or description of the to-do item
        // [Required] ensures the user must enter a value
        // [StringLength(100)] limits the input to 100 characters
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        // A flag indicating whether this task is completed
        public bool IsDone { get; set; }
    }
}
