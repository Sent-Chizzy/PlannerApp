using System;
using System.ComponentModel.DataAnnotations;

namespace PlannerApp.Models
{
    /// <summary>
    /// Enum for difficulty ranking: 1=Easy, 2=Moderate, 3=Hard.
    /// </summary>
    public enum Difficulty
    {
        Easy = 1,
        Moderate = 2,
        Hard = 3
    }

    // Represents a single to-do/task in the planner application
    public class TodoItem
    {
        // Primary key
        [Key]
        public int Id { get; set; }

        // Short title (required, max length 100)
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        // Longer description or notes (optional, max length 500)
        [StringLength(500)]
        public string Description { get; set; }

        // Due date (optional, date-only)
        public DateTime? DueDate { get; set; }

        // Indicates whether the task is completed
        public bool IsDone { get; set; }

        // Difficulty ranking (required)
        [Required]
        public Difficulty DifficultyLevel { get; set; }

        // Progress percentage: 0–100, in 10% increments
        // When Progress == 100, we will treat the task as completed.
        [Range(0, 100)]
        public int Progress { get; set; } = 0;
    }
}
