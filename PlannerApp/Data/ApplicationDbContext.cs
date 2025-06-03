using Microsoft.EntityFrameworkCore; // EF Core namespace
using PlannerApp.Models;                // Our models namespace

namespace TodoApp.Data
{
    // ApplicationDbContext is the EF Core "gateway" to our database
    public class ApplicationDbContext : DbContext
    {
        // The constructor receives configuration options from Program.cs
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // This DbSet<TodoItem> property tells EF Core
        // to create a table named "TodoItems" in the database.
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
